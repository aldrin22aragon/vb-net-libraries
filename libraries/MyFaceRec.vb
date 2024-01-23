Imports System.IO
Imports Neurotec.Biometrics
Imports Neurotec.Biometrics.Client
Imports Neurotec.Biometrics.Gui
Imports Neurotec.Licensing



Public Class MyFaceRec
   Public tmpExtention As String = ".enc" ' must be with dot at beginning. 
   Public Const environmentFolderName As String = "Face recognition"
   Public environmentFolderPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), environmentFolderName)
   Public capturedFaceInfo As New MyFaceRecReturnStatement
   Public getImageOnly As Boolean = False
   '
   Friend WithEvents Timer1 As New Timer
   Event FaceDetected(sender As Object, st As MyFaceRecReturnStatement)
   '
   Public FV As New NFaceView
   Public minimumFaceMatchingScore As Integer = 0
   Public CameraSubject1 As New NSubject()
   Public faceRecClient As NBiometricClient
   Public _biometricClient2 As New NBiometricClient() With {.BiometricTypes = NBiometricType.Face, .UseDeviceManager = False, .FacesMatchingSpeed = NMatchingSpeed.High}
   Public BiometricTask As NBiometricTask
   Public captureOpions As NBiometricCaptureOptions = NBiometricCaptureOptions.Manual
   '
   ''' <summary>
   ''' Ginagamit lang ito para maka kuha ng Byte data na naCapture na mukha
   ''' using capTureByte function. Sample declaration: Friend WithEvents faceRec As MyFaceRec, on form load=>  faceRec = New MyFaceRec(fv)
   ''' important to check first if license is valid 
   ''' use this to check MyFaceRec.RegisterLicenses()
   ''' </summary>
   ''' <param name="fv">NFaceView na image viewer na dapat i add sa form</param>
   Sub New(fv As NFaceView, settings As BiometricsSettings, Optional getImageOnly As Boolean = True)
      DeleteTemporaryImagesCreatedByCapturing()
      faceRecClient = GetNewBiometricClient(settings)
      Me.getImageOnly = getImageOnly
      Me.captureOpions = captureOpions
      Me.FV = fv
      StartBuffer()
   End Sub
   ''' <summary>
   ''' Ginagamit ito para mag detect ng mukha.
   ''' Once naka detect na ng mukha, mag trigger ang event na FaceDetected.
   ''' Sample declaration: Friend WithEvents faceRec As MyFaceRec, on form load=>  faceRec = New MyFaceRec(fv, getTasksFaces())
   ''' important to check first if license is valid 
   ''' use this to check MyFaceRec.RegisterLicenses()
   ''' </summary>
   ''' <param name="fv">NFaceView na image viewer na dapat i add sa form</param>
   ''' <param name="list">NBiometricTask Dito i store lahat ng mga faces. Kase dito naka base yung 'FaceDetected' event kung existing na</param>
   Sub New(fv As NFaceView, settings As BiometricsSettings, list As List(Of ID_and_faceDataAsBlob))
      DeleteTemporaryImagesCreatedByCapturing()
      faceRecClient = GetNewBiometricClient(settings)
      minimumFaceMatchingScore = settings.MinimumFaceMatchScore
      Dim tmpTask As New NBiometricTask(NBiometricOperations.Enroll)
      For Each idByt As ID_and_faceDataAsBlob In list
         If idByt.faceByts IsNot Nothing Then
            Dim face As NSubject = NSubject.FromMemory(Convert.FromBase64String(idByt.faceByts))
            'NSubject.FromMemory(NBuffer.FromArray(face_data1))
            face.Id = idByt.id
            tmpTask.Subjects.Add(face)
         End If
      Next
      Me.captureOpions = captureOpions
      Me.FV = fv
      Me.BiometricTask = tmpTask
      StartDetectingFace()
   End Sub

   Public Sub OverWriteBiometricktasks(list As List(Of ID_and_faceDataAsBlob))
      Dim tmpTask As New NBiometricTask(NBiometricOperations.Enroll)
      For Each idByt As ID_and_faceDataAsBlob In list
         If idByt.faceByts IsNot Nothing Then
            Dim face As NSubject = NSubject.FromMemory(Convert.FromBase64String(idByt.faceByts))
            'NSubject.FromMemory(NBuffer.FromArray(face_data1))
            face.Id = idByt.id
            tmpTask.Subjects.Add(face)
         End If
      Next
      Me.BiometricTask = tmpTask
   End Sub

   Sub DeleteTemporaryImagesCreatedByCapturing()
      If IO.Directory.Exists(environmentFolderPath) Then
         Dim ss As String() = IO.Directory.GetFiles(environmentFolderPath, "*" & tmpExtention)
         For Each i As String In ss
            If Not _FileInUsed(i) Then
               IO.File.Delete(i)
            End If
         Next
      End If
   End Sub

   Function GetNewBiometricClient(settings As BiometricsSettings) As NBiometricClient
      Dim res As New NBiometricClient()
      With res
         .BiometricTypes = settings.BiometricTypes
         .FacesMaximalRoll = settings.FacesMaximalRoll
         .FacesMaximalYaw = settings.FacesMaximalYaw
         .FacesMatchingSpeed = settings.FacesMatchingSpeed
         .FacesTemplateSize = settings.FacesTemplateSize
         .FacesLivenessMode = settings.FacesLivenessMode
         .FacesLivenessThreshold = settings.FacesLivenessThreshold
         .FacesLivenessBlinkTimeout = settings.FacesLivenessBlinkTimeout
         .FacesQualityThreshold = settings.FacesQualityThreshold
         .FacesConfidenceThreshold = settings.FacesConfidenceThreshold
         .UseDeviceManager = settings.UseDeviceManager
         .FacesDetectAllFeaturePoints = settings.FacesDetectAllFeaturePoints
      End With
      Return res
   End Function

   Sub StartDetectingFace()
      Dim face As New NFace() With {.CaptureOptions = captureOpions}
      CameraSubject1.Faces.Add(face)
      FV.Face = face
      faceRecClient.CaptureAsync(CameraSubject1)
      Timer1.Enabled = True
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Sub

   Async Sub StopDetectingFace()
      If getImageOnly Then
         stopTheCapturing = True
         faceRecClient.ForceStart()
         FV.Face = Nothing
      Else
         Timer1.Enabled = False
         faceRecClient.Force()
         FV.Face = Nothing
      End If
      Await faceRecClient.ClearAsync()
      CameraSubject1.Faces(0).Image = Nothing
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Sub
   Dim stopTheCapturing As Boolean = False

   Async Sub StartBuffer()
      stopTheCapturing = False
      While True
         If stopTheCapturing Then
            Exit While
         End If
         Dim face As New NFace() With {.CaptureOptions = NBiometricCaptureOptions.Manual}
         CameraSubject1.Faces.Add(face)
         FV.Face = face 'CameraSubject1.Faces(0) 'face
         Dim stat = Await faceRecClient.CaptureAsync(CameraSubject1)
         If stat = NBiometricStatus.Ok Then
            Dim facesCount As Integer = CameraSubject1.Faces.Count
            Dim recordsCount As Integer = CameraSubject1.GetTemplate().Faces.Records.Count
            Dim bt As Bitmap = CameraSubject1.Faces(facesCount - 1).Image.ToBitmap.Clone()
            Dim img As Bitmap = CameraSubject1.Faces(facesCount - 1).Image.ToBitmap.Clone()
            capturedFaceInfo.success = True

            'Dim buffer As Neurotec.IO.NBuffer = freg.fs1.GetTemplateBuffer
            'Dim bufferByt As Byte() = buffer.ToArray
            'Faces.face_data1 = bufferByt

            capturedFaceInfo.capturedBase64 = Convert.ToBase64String(CameraSubject1.GetTemplateBuffer.ToArray)
            capturedFaceInfo.quality = CameraSubject1.GetTemplate().Faces.Records(recordsCount - 1).Quality
            capturedFaceInfo.message = "Success"
            If Not Directory.Exists(environmentFolderPath) Then MkDir(environmentFolderPath)
            Dim counter As Integer = 1
            capturedFaceInfo.imageTmpPath = IO.Path.Combine(environmentFolderPath, ("tmpImage" & counter & tmpExtention))
            While _FileInUsed(capturedFaceInfo.imageTmpPath)
               capturedFaceInfo.imageTmpPath = IO.Path.Combine(environmentFolderPath, ("tmpImage" & counter & tmpExtention))
               counter += 1
            End While
            Using memory As MemoryStream = New MemoryStream()
               Using fs As FileStream = New FileStream(capturedFaceInfo.imageTmpPath, FileMode.Create, FileAccess.ReadWrite)
                  img.Save(memory, Imaging.ImageFormat.Jpeg)
                  Dim bytes As Byte() = memory.ToArray()
                  fs.Write(bytes, 0, bytes.Length)
                  img.Dispose()
                  img = Nothing
               End Using
            End Using
            GC.Collect()
            GC.WaitForPendingFinalizers()
         Else
            capturedFaceInfo.success = False
            capturedFaceInfo.message = ""
         End If
      End While
   End Sub
   Public Function CaptureFaceInfo() As MyFaceRecReturnStatement
      capturedFaceInfo = New MyFaceRecReturnStatement() With {.message = "waiting"}
      faceRecClient.ForceStart()
      While capturedFaceInfo.message = "waiting"
         Application.DoEvents()
      End While
      Return capturedFaceInfo
   End Function
   Public reading As Boolean = False
   Private Async Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
      If Not reading Then
         reading = True
         For Each f As NFace In CameraSubject1.Faces
            If f.Image IsNot Nothing Then
               Dim faces As New List(Of NFace)
               Dim face As NFace = Await faceRecClient.DetectFacesAsync(f.Image)
               For i As Integer = 0 To face.Objects.Count - 1
                  Dim nla As New NLAttributes With {.BoundingRect = face.Objects(i).BoundingRect}
                  Dim g = NFace.FromImageAndAttributes(face.Image, nla)
                  faces.Add(g)
               Next
               If faces.Count > 0 Then
                  Dim tmpFaces As NFace() = faces.ToArray
                  For Each iFace As NFace In tmpFaces
                     Dim subject As New NSubject
                     subject.Faces.Add(iFace)
                     Await _biometricClient2.ClearAsync()
                     Dim taskAsync = Await _biometricClient2.PerformTaskAsync(BiometricTask)
                     Dim ret As New MyFaceRecReturnStatement With {
                        .success = False
                     }
                     If taskAsync.Status = NBiometricStatus.Ok Then
                        Dim identifyTask = Await _biometricClient2.IdentifyAsync(subject)
                        If subject.GetTemplate().Faces IsNot Nothing Then
                           Dim quality As Integer = subject.GetTemplate().Faces.Records(0).Quality
                           If identifyTask = NBiometricStatus.Ok OrElse identifyTask = NBiometricStatus.MatchNotFound Then
                              Dim m = (From res In subject.MatchingResults Where res.Score >= minimumFaceMatchingScore
                                       Order By res.Score Descending Take 1 Select {res.Id, res.Score}).FirstOrDefault
                              Dim scores As New List(Of Integer)
                              For Each j As NMatchingResult In subject.MatchingResults
                                 scores.Add(j.Score)
                              Next
                              If m IsNot Nothing Then
                                 ret.ID = m(0)
                                 ret.success = True
                                 ret.message = "✓ " & m(0)
                                 ret.otherMatches = subject.MatchingResults
                                 ret.quality = quality
                              Else
                                 If subject.MatchingResults.Count > 0 Then
                                    ret.message = "Face is not clear. Please face properly to the camera. (" & scores(0).ToString & ")"
                                 Else
                                    ret.message = "Face can't find any similar. Or face is not yet registered."
                                 End If
                              End If

                           Else
                              ret.message = "Please face properly to the camera."
                           End If
                        Else
                           ret.message = "Please face properly to the camera..."
                        End If
                     Else
                        ret.message = "Error on biometrics. Please contact tech."
                     End If
                     RaiseEvent FaceDetected(Me, ret)
                  Next
               End If
            End If
         Next
         reading = False
      End If
   End Sub

#Region "Functions"
   Public Shared Function IsConnectedToInternet() As BoolReasonClass
      Dim host As Net.IPAddress = Net.IPAddress.Parse("8.8.8.8")
      Dim result As New BoolReasonClass
      Dim p As New Net.NetworkInformation.Ping
      Try
         Dim reply As Net.NetworkInformation.PingReply = p.Send(host, 500)
         If reply.Status = Net.NetworkInformation.IPStatus.Success Then
            result.success = True
         Else
            result.success = False
            result.reason = "Unable to connect to internet. Internet is required"
         End If
      Catch ex As Exception
         result.success = False
         result.reason = "Unable to connect to internet. Internet is required"
      End Try
      Return result
   End Function
   Public Shared Function RegisterLicenses() As BoolReasonClass
      Dim res As New BoolReasonClass
      res.success = True
      Dim lisFOlder As String = IO.Path.Combine(Application.StartupPath, "Activation\Licenses")
      Dim licFiles As String()
      Try
         licFiles = IO.Directory.GetFiles(lisFOlder, "*.lic")
      Catch ex As Exception
         ex = ex
      End Try
      Dim licCons As New List(Of String)
      For Each licFile As String In licFiles
         Dim test As String = IO.File.ReadAllText(licFile)
         test = test.Trim()
         licCons.Add(test)
      Next
      Dim licences As String() = licCons.ToArray
      For Each lic As String In licences
         NLicense.Add(lic)
      Next
      Const Components As String = "Biometrics.FaceExtraction,Biometrics.FaceMatching,Biometrics.FaceDetection,Devices.Cameras" ',Biometrics.FaceSegmentsDetection"
      For Each component As String In Components.Split(New Char() {","}, StringSplitOptions.RemoveEmptyEntries)
         If NLicense.ObtainComponents("/local", "5000", component.Trim) = False Then
            res.success = False
            res.reason = String.Concat(res.reason, vbNewLine, component & ": Component License is not activated")
         End If
      Next
      If Not res.success Then
         res.reason = String.Concat(res.reason, vbNewLine, "Need to activate License in: ", IO.Path.GetDirectoryName(lisFOlder))
      End If
      Return res
   End Function
   Public Shared Function _FileInUsed(ByVal FilePath As String, Optional displayMsg As Boolean = False) As Boolean
      Try
         If IO.File.Exists(FilePath) Then
            Dim FS As IO.FileStream = IO.File.Open(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
            FS.Close()
            FS.Dispose()
            FS = Nothing
            Return False
         Else
            Return False
         End If
      Catch ioEX As IOException
         If displayMsg Then
            MsgBox("File " & vbNewLine & FilePath & vbNewLine & " is In Use!", MsgBoxStyle.Critical, "")
         End If
         Return True
      Catch ex As Exception
         If displayMsg Then
            MsgBox("Unknown error occured!", MsgBoxStyle.Critical, "")
         End If
         Return True
      End Try
   End Function
#End Region
#Region "Classess"
   Public Class BoolReasonClass
      Public success As Boolean = False
      Public reason As String = ""
      Public data As Object
   End Class
   Class ID_and_faceDataAsBlob
      Public id As String = ""
      Public faceByts As String = ""
      Sub New(id As String, bytes As Object)
         Me.id = id
         Me.faceByts = bytes
      End Sub
      Sub New()
      End Sub
   End Class

   Class MyFaceRecReturnStatement
      Public success As Boolean = False
      Public message As String = ""
      Public capturedBase64 As String
      Public quality As Integer = 0
      Public imageTmpPath As String = ""
      Public ID As String = ""
      Public otherMatches As NSubject.MatchingResultCollection
      'Public nImage As Neurotec.Images.NImage = Nothing
      'Public image As Bitmap = Nothing
      'Public data As Object
   End Class
   Public Class BiometricsSettings
      Public BiometricTypes As NBiometricType = NBiometricType.Face
      Public FacesMaximalRoll As Integer = 15
      Public FacesMaximalYaw As Integer = 15
      Public FacesMatchingSpeed As NMatchingSpeed = NMatchingSpeed.Low
      Public FacesTemplateSize As NTemplateSize = NTemplateSize.Medium
      Public FacesLivenessMode As NLivenessMode = NLivenessMode.None
      Public FacesLivenessThreshold As Integer = 10
      Public FacesLivenessBlinkTimeout As Integer = 10
      Public FacesQualityThreshold As Integer = 65
      Public FacesConfidenceThreshold As Integer = 65
      Public UseDeviceManager As Boolean = True
      Public FacesDetectAllFeaturePoints As Boolean = False
      '
      Public MinimumFaceMatchScore As Integer = 65
      Public UserReloginHours As Integer = 1
      Public SingeImageRecogNition As Boolean = False
   End Class
#End Region
End Class

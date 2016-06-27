using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WindowsServer.Configuration;
using WindowsServer.Log;
using WindowsServer.Web;

namespace Collaboration.TaskForce.Service.Models
{
    public static class AttachmentStorage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static string _storageRoot = null;
        private static string _previewStorageRoot = null;
        private static bool _isUseAzureStorage = false;
        private static string _defaultEndpointsProtocol = string.Empty;
        private static string _storageAccountName = string.Empty;
        private static string _storageAccountKey = string.Empty;
        private static string _blobEndpoint = string.Empty;
        private static string _storageContainerName = string.Empty;
        private static string _storagePreviewContainerName = string.Empty;
        static AttachmentStorage()
        {
            bool.TryParse(ConfigurationCenter.Global["UseAzureStorage"], out _isUseAzureStorage);
            _defaultEndpointsProtocol = ConfigurationCenter.Global["DefaultEndpointsProtocol"] ?? "http";
            _storageAccountName = ConfigurationCenter.Global["StorageAccountName"];
            _storageAccountKey = ConfigurationCenter.Global["StorageAccountKey"];
            _blobEndpoint = string.Format("http://{0}.blob.core.chinacloudapi.cn/", _storageAccountName);
            _storageContainerName = ConfigurationCenter.Global["ZigeAttachmentContainer"] ?? "storage";
            _storagePreviewContainerName = ConfigurationCenter.Global["ZigeAttachmentPreviewContainer"] ?? "storagepreview";
            if (!_isUseAzureStorage)
            {
                _storageRoot = ConfigurationCenter.Global["AttachmentStorageRoot"];
                if (_storageRoot.StartsWith("~"))
                {
                    _storageRoot = HttpContext.Current.Server.MapPath(_storageRoot);
                }
                if (!Directory.Exists(_storageRoot))
                {
                    Directory.CreateDirectory(_storageRoot);
                }
                _logger.Info("StorageRoot: " + _storageRoot);

                _previewStorageRoot = ConfigurationCenter.Global["AttachmentPreviewStorageRoot"];
                if (_previewStorageRoot.StartsWith("~"))
                {
                    _previewStorageRoot = HttpContext.Current.Server.MapPath(_previewStorageRoot);
                }
                if (!Directory.Exists(_previewStorageRoot))
                {
                    Directory.CreateDirectory(_previewStorageRoot);
                }
                _logger.Info("PreviewStorageRoot: " + _previewStorageRoot);
            }
        }

        public static void AddAttachment(Guid corporationId, Guid taskId, Guid attachmentId, string mimeType, Stream dataStream)
        {
            if (!_isUseAzureStorage)
            {
                var filePath = GetAttachmentFilePath(corporationId, taskId, attachmentId, mimeType);
                var directoryName = Path.GetDirectoryName(filePath);
                if (!System.IO.Directory.Exists(directoryName))
                {
                    System.IO.Directory.CreateDirectory(directoryName);
                }
                using (var fileStream = File.Create(filePath))
                {
                    dataStream.CopyTo(fileStream);
                }
            }
            else
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(string.Format("BlobEndpoint={0};DefaultEndpointsProtocol={1};AccountName={2};AccountKey={3}", _blobEndpoint, _defaultEndpointsProtocol, _storageAccountName, _storageAccountKey));
                CloudBlobClient blobClient = new CloudBlobClient(_blobEndpoint, cloudStorageAccount.Credentials);//cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(_storageContainerName);
                blobContainer.CreateIfNotExist();
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                blobContainer.SetPermissions(containerPermissions);
                var attaFileName = GetAttachmentFileName(attachmentId, mimeType);
                CloudBlob blob = blobContainer.GetBlobReference(attaFileName);
                blob.UploadFromStream(dataStream);
            }
        }

        public static void AddAttachmentPreview(Guid corporationId, Guid taskId, Guid attachmentId, string previewParameters, string mimeType, Stream dataStream)
        {
            var filePath = GetAttachmentPreviewFilePath(corporationId, taskId, attachmentId, previewParameters, mimeType);
            var directoryName = Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(directoryName))
            {
                System.IO.Directory.CreateDirectory(directoryName);
            }
            using (var fileStream = File.Create(filePath))
            {
                dataStream.CopyTo(fileStream);
            }
        }

        public static Stream GetAttachmentStream(Guid corporationId, Guid taskId, Guid attachmentId, string mimeType)
        {
            var filePath = GetAttachmentFilePath(corporationId, taskId, attachmentId, mimeType);
            if (!_isUseAzureStorage)
            {
                if (File.Exists(filePath))
                {
                    return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                return null;
            }
            else
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(string.Format("BlobEndpoint={0};DefaultEndpointsProtocol={1};AccountName={2};AccountKey={3}", _blobEndpoint, _defaultEndpointsProtocol, _storageAccountName, _storageAccountKey));
                CloudBlobClient blobClient = new CloudBlobClient(_blobEndpoint, cloudStorageAccount.Credentials);//cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(_storageContainerName);
                blobContainer.CreateIfNotExist();
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                blobContainer.SetPermissions(containerPermissions);
                var attaFileName = GetAttachmentFileName(attachmentId, mimeType);
                CloudBlob blob = blobContainer.GetBlobReference(attaFileName);
                var dataStream = new MemoryStream();
                blob.DownloadToStream(dataStream);

                return dataStream;
            }
        }

        public static FileStream GetAttachmentPreviewStream(Guid corporationId, Guid taskId, Guid attachmentId, string previewParameters, string mimeType)
        {
            var filePath = GetAttachmentPreviewFilePath(corporationId, taskId, attachmentId, previewParameters, mimeType);
            if (File.Exists(filePath))
            {
                return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            return null;
        }

        public static string GetAttachmentFilePath(Guid corporationId, Guid taskId, Guid attachmentId, out string mimeType)
        {
            var filePath = GetAttachmentFilePath(corporationId, taskId, attachmentId, string.Empty);
            if (!_isUseAzureStorage)
            {
                var files = System.IO.Directory.GetFiles(
                                                     Path.GetDirectoryName(filePath),
                                                     Path.GetFileNameWithoutExtension(filePath) + ".*");
                if (files.Length > 0)
                {
                    var file = files[0];
                    var extension = Path.GetExtension(file).TrimStart('.');
                    mimeType = DecodeMimeType(extension);
                    return file;
                }
                mimeType = string.Empty;
            }
            else
            {
                mimeType = string.Empty;
            }

            return null;
        }

        public static string GetAttachmentFilePath(Guid corporationId, Guid taskId, Guid attachmentId, string mimeType)
        {
            var taskIdString = taskId.ToString("N");
            var filePath = string.Empty;
            if (!_isUseAzureStorage)
            {
                filePath = string.Format(
                "{0}\\{1}\\{2}\\{3}\\{4}.{5}",
                _storageRoot,
                CorporationUserUtility.ToCorporationIdShortString(corporationId),
                taskIdString.Substring(0, 2),
                taskIdString,
                attachmentId.ToString("N"),
                EncodeMimeType(mimeType));
            }
            else
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(string.Format("BlobEndpoint={0};DefaultEndpointsProtocol={1};AccountName={2};AccountKey={3}", _blobEndpoint, _defaultEndpointsProtocol, _storageAccountName, _storageAccountKey));
                CloudBlobClient blobClient = new CloudBlobClient(_blobEndpoint, cloudStorageAccount.Credentials);//cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(_storageContainerName);

                filePath = UrlUtility.Combine(Path.Combine(_blobEndpoint, _storageContainerName), GetAttachmentFileName(attachmentId, mimeType));
            }

            return filePath;
        }

        public static string GetAttachmentFileName(Guid attachmentId, string mimeType)
        {
            return string.Format("{0}.{1}", attachmentId.ToString("N"), EncodeMimeType(mimeType));
        }

        public static string GetAttachmentPreviewFilePath(Guid corporationId, Guid taskId, Guid attachmentId, string previewParameters, string mimeType)
        {
            var taskIdString = taskId.ToString("N");
            var filePath = string.Format(
                "{0}\\{1}\\{2}\\{3}\\{4}.{5}.{6}",
                _previewStorageRoot,
                CorporationUserUtility.ToCorporationIdShortString(corporationId),
                taskIdString.Substring(0, 2),
                taskIdString,
                attachmentId.ToString("N"),
                previewParameters,
                EncodeMimeType(mimeType));
            return filePath;
        }

        private static string EncodeMimeType(string mimeType)
        {
            var s = mimeType.Trim();
            s = s.Replace('/', '(');
            s = s.Replace('\\', ')');
            s = s.Replace('-', '_');
            s = s.Replace('.', '^');
            s = s.Replace('*', '!');
            return s;
        }

        private static string DecodeMimeType(string fileExtension)
        {
            var s = fileExtension.Trim();
            s = s.Replace('(', '/');
            s = s.Replace(')', '\\');
            s = s.Replace('_', '-');
            s = s.Replace('^', '.');
            s = s.Replace('!', '*');
            return s;
        }
    }

    public static class CorporationUserUtility
    {
        public static Guid ConvertUserIdToCorporationId(Guid userId)
        {
            var bytes = userId.ToByteArray();
            bytes[8] = 0;
            bytes[9] = 0;
            bytes[10] = 0;
            bytes[11] = 0;
            bytes[12] = 0;
            bytes[13] = 0;
            bytes[14] = 0;
            bytes[15] = 0;
            return new Guid(bytes);
        }

        public static Guid GenerateUserId(Guid corporationId)
        {
            var corporationBytes = corporationId.ToByteArray();
            var userBytes = Guid.NewGuid().ToByteArray();
            userBytes[0] = corporationBytes[0];
            userBytes[1] = corporationBytes[1];
            userBytes[2] = corporationBytes[2];
            userBytes[3] = corporationBytes[3];
            userBytes[4] = corporationBytes[4];
            userBytes[5] = corporationBytes[5];
            userBytes[6] = corporationBytes[6];
            userBytes[7] = corporationBytes[7];
            return new Guid(userBytes);
        }

        public static Guid GenerateCorporationId()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            bytes[8] = 0;
            bytes[9] = 0;
            bytes[10] = 0;
            bytes[11] = 0;
            bytes[12] = 0;
            bytes[13] = 0;
            bytes[14] = 0;
            bytes[15] = 0;
            return new Guid(bytes);
        }

        public static Guid ParseCorporationId(string s)
        {
            if (s.Length < 32)
            {
                return Guid.ParseExact(s + "0000000000000000", "N");
            }
            return Guid.Parse(s);
        }

        public static string ToCorporationIdShortString(Guid corporationId)
        {
            return corporationId.ToString("N").Substring(0, 16);
        }

        public static bool IsSameCorporationUser(Guid corporationId, Guid userId)
        {
            return (corporationId == CorporationUserUtility.ConvertUserIdToCorporationId(userId));
        }


    }
}
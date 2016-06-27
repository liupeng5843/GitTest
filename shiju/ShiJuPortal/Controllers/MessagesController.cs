using Collaboration.TaskForce.Service.Models;
using ShiJu.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace ShiJu.Portal.Controllers
{
    public class MessagesController : ApiController
    {
        [Route("Messages/{partyId}/Logs/Attachments/{attachmentId}")]
        public HttpResponseMessage GetMessageAttachment(Guid partyId, Guid attachmentId)
        {
            string mimeType = "audio/collaboration.amr";//hard code for current app cuz just only this type of attachment exists;
            //var filePath = AttachmentStorage.GetAttachmentFilePath(ZigeApp.CorporationId, parentId, attachmentId, out mimeType);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            using (Stream stream = AttachmentStorage.GetAttachmentStream(ShiJuApp.CorporationId, partyId, attachmentId, mimeType))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = attachmentId.ToString();
            }
            return response;
        }
    }
}
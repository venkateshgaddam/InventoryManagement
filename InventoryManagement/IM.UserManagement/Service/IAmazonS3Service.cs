using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    public interface IAmazonS3Service
    {
        Task<PutObjectResponse> UploadFileAsync(Stream stream, string bucketName, string keyName);
    }
}

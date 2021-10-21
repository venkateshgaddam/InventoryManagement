using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IM.UserManagement.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly IAmazonS3 amazonS3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amazonS3"></param>
        public AmazonS3Service(IAmazonS3 amazonS3)
        {
            this.amazonS3 = amazonS3;
        }



        #region ServiceMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bucketName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public async Task<PutObjectResponse> UploadFileAsync(Stream stream, string bucketName, string keyName)
        {
            PutObjectRequest putObjectRequest = new()
            {
                InputStream = stream,
                BucketName = bucketName,
                Key = keyName,
                TagSet = new List<Tag>
                {
                    {new Tag{Key="fileName",Value=keyName} }
                }
            };

            PutObjectResponse response = await amazonS3.PutObjectAsync(putObjectRequest);

            return response;
        }



        #endregion
    }
}

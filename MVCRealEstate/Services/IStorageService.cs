using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MVCRealEstate.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(Stream? stream, int width = 800, int height = 600);
    }

    public class StorageService : IStorageService
    {
        private readonly IConfiguration configuration;

        public StorageService(
            IConfiguration configuration
            )
        {
            this.configuration = configuration;
        }
        public async Task<string> UploadAsync(Stream? stream, int width = 800, int height = 600)
        {
            if (stream is null)
                return default;

            var image = await Image.LoadAsync(stream);

            image.Mutate(p => p.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Pad
            }));

            if (configuration.GetValue<bool>("UseStorage"))
            {
                using var ms = new MemoryStream();
                image.Save(ms, JpegFormat.Instance);

                var s3Client = new AmazonS3Client(
                    configuration.GetValue<string>("Storage:AccessKey"),
                    configuration.GetValue<string>("Storage:SecretKey"),
                    RegionEndpoint.USEast1);

                using var transferUtility = new TransferUtility(s3Client);
                var bucketFileName = $"{Path.GetRandomFileName()}.jpg";
                transferUtility.Upload(ms, configuration.GetValue<string>("Storage:BucketName"), bucketFileName);

                return $"http://{configuration.GetValue<string>("Storage:BucketName")}.s3.amazonaws.com/{bucketFileName}";

            }
            else
            {
                return image.ToBase64String(JpegFormat.Instance);
            }

            
        }
    }
}

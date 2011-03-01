namespace Appleseed.Framework.DataTypes
{
    public class ImageDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long GalleryId { get; set; }
        public string GalleryName { get; set; }
        public string GalleryDescription { get; set; }
        public string FileExtension { get; set; }
        public string CompletePath { get; set; }
    }
}

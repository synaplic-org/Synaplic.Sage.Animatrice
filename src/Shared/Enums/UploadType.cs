using System.ComponentModel;

namespace Uni.Scan.Shared.Enums
{
    public enum UploadType : byte
    {
        

        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }
}
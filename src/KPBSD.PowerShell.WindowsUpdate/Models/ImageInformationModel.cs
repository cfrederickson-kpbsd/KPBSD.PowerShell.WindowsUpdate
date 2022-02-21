using System;

namespace KPBSD.PowerShell.WindowsUpdate
{
    // I manually mapped this one and don't actually know the COM guid.
    // [ModelForGuid("$ComGuid")]
    public class ImageInformationModel : Model, IEquatable<ImageInformationModel>
    {
        private readonly dynamic _comObject;
        public ImageInformationModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public string AltText { get { return _comObject.AltText; } }
        public long Height { get { return _comObject.Height; } }
        public string Source { get { return _comObject.Source; } }
        public long Width { get { return _comObject.Width; } }

        public bool Equals(ImageInformationModel other)
        {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other)
        {
            return other is ImageInformationModel && Equals((ImageInformationModel)other);
        }
        public override int GetHashCode()
        {
            return this._comObject.GetHashCode();
        }
    }
}
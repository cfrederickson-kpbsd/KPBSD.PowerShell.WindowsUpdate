namespace KPBSD.PowerShell.WindowsUpdate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Mapped from 81ddc1b8-9d35-47a6-b471-5b80f519223b.
    /// </summary>
    [ModelForGuid("81ddc1b8-9d35-47a6-b471-5b80f519223b")]
    public class CategoryModel : Model, IEquatable<CategoryModel>
    {
        private readonly dynamic _comObject;
        internal CategoryModel(object comObject)
        {
            this._comObject = comObject;
        }
        public object ComObject { get { return _comObject; } }
        public string CategoryID { get { return _comObject.CategoryID; } }
		public CategoryModel[] Children { get { return ToModelArray(_comObject.Children); } }
		public string Description { get { return _comObject.Description; } }
		public ImageInformationModel Image { get { return (ImageInformationModel)CreateModel(_comObject.Image); } }
		public string Name { get { return _comObject.Name; } }
		public int Order { get { return _comObject.Order; } }
		public CategoryModel Parent { get { return (CategoryModel)CreateModel(_comObject.Parent); } }
		public string Type { get { return _comObject.Type; } }
		public UpdateModel[] Updates { get { return ToModelArray(_comObject.Updates); } }

        public bool Equals(CategoryModel other) {
            return other != null && Equals(this._comObject, other._comObject);
        }
        public override bool Equals(object other) {
            return other is CategoryModel && this.Equals((CategoryModel)other);
        }
        public override int GetHashCode() {
            return this._comObject.GetHashCode();
        }

        public override string ToString()
        {
            return $"Windows Update Category '{Name}' (Id '{CategoryID}')";
        }
    }
}

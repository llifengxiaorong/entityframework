// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure
{
    using System.Data.Entity.Utilities;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Loads or saves models from/into .edmx files at a specified location.
    /// </summary>
    public class DefaultDbModelStore : DbModelStore
    {
        private const string FileExtension = ".edmx";

        private readonly string _location;

        /// <summary>
        /// Initializes a new DefaultDbModelStore instance.
        /// </summary>
        /// <param name="location">The parent directory for the .edmx files.</param>
        public DefaultDbModelStore(string location)
        {
            Check.NotEmpty(location, "location");

            _location = location;
        }

        /// <summary>
        /// Gets the location of the .edmx files.
        /// </summary>
        public string Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Loads or a model from the store.
        /// </summary>
        /// <param name="contextType">The type of context representing the model.</param>
        /// <returns>The loaded metadata model.</returns>
        public override DbCompiledModel TryLoad(Type contextType)
        {
            var filePath = GetFilePath(contextType);

            if (!File.Exists(filePath))
            {
                return null;
            }

            using (var reader = XmlReader.Create(filePath))
            {
                var defaultSchema = GetDefaultSchema(contextType);

                return EdmxReader.Read(reader, defaultSchema);
            }
        }

        /// <summary>
        /// Saves a model to the store.
        /// </summary>
        /// <param name="contextType">The type of context representing the model.</param>
        /// <param name="model">The metadata model to save.</param>
        public override void Save(Type contextType, DbModel model)
        {
            using (var writer = XmlWriter.Create(GetFilePath(contextType), 
                new XmlWriterSettings
                    {
                        Indent = true
                    }))
            {
                EdmxWriter.WriteEdmx(model, writer);
            }
        }

        /// <summary>
        /// Gets the path of the .edmx file corresponding to the specified context type.
        /// </summary>
        /// <param name="contextType">A context type.</param>
        /// <returns>The .edmx file path.</returns>
        protected virtual string GetFilePath(Type contextType)
        {
            var fileName = contextType.FullName + FileExtension;

            return Path.Combine(_location, fileName);
        }
    }
}

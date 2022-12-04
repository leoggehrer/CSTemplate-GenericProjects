using System.Text.Json;

namespace Repository.Logic
{
    public class GenericRepository<TModel> : IDisposable
        where TModel : Contracts.IIdentifyable, new()
    {
        #region Fields
        private List<TModel> modelList = new();
        #endregion Fields

        #region Properties
        protected virtual string FilePath { get; set; } = $"{typeof(TModel).Name}.json";
        #endregion Properties

        public GenericRepository()
        {
            Load(FilePath);
        }
        public GenericRepository(string filePath)
        {
            FilePath = filePath;
            Load(FilePath);
        }

        #region Create
        public virtual TModel Create() => new TModel();
        public virtual Task<TModel> CreateAsync() => Task.Run(() => Create());
        #endregion Create

        #region Get
        public virtual TModel? GetById(int id) => modelList.FirstOrDefault(m => m.Id == id);
        public virtual Task<TModel?> GetByIdAsync(int id) => Task.Run(() => GetById(id)); 
        public virtual TModel[] GetAll()
        {
            return modelList.Where(m => m is TModel)
                            .ToArray();
        }
        public virtual Task<TModel[]> GetAllAsync() => Task.Run(() => GetAll());
        #endregion Get

        #region Insert
        public virtual TModel Insert(TModel model)
        {
            if (modelList.Any())
            {
                model.Id = modelList.Max(m => m.Id) + 1;
            }
            else
            {
                model.Id = 1;
            }
            modelList.Add(model);
            return model;
        }
        public virtual Task InsertAsync(TModel model)
        {
            return Task.Run(() => Insert(model));
        }
        #endregion Insert

        #region Update
        public virtual bool Update(TModel model)
        {
            var listModel = modelList.FirstOrDefault(m => m.Id == model.Id);

            if (listModel != null)
            {
                modelList.Remove(listModel);
                modelList.Add(model);
            }
            return listModel != null;
        }
        public virtual Task<bool> UpdateAsync(TModel model)
        {
            return Task.Run(() => Update(model));
        }
        #endregion Update

        #region Delete
        public virtual void Delete(int id)
        {
            var listModel = modelList.FirstOrDefault(m => m.Id == id);

            if (listModel != null)
            {
                modelList.Remove(listModel);
            }
        }
        public virtual Task DeleteAsync(int id)
        {
            return Task.Run(() => Delete(id));
        }
        #endregion Delete

        #region SaveChanges
        public virtual void SaveChanges()
        {
            Save(FilePath);
        }
        public virtual Task SaveChangesAsync()
        {
            return Task.Run(() => SaveChanges());
        }
        #endregion SaveChanges

        #region Load and save
        internal virtual void Save(string filePath)
        {
            var jsonData = JsonSerializer.Serialize<TModel[]>(modelList.ToArray());
            var directoryName = Path.GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(directoryName) == false
                && Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            File.WriteAllText(filePath, jsonData);
        }
        internal virtual void Load(string filePath)
        {
            modelList.Clear();
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                var models = JsonSerializer.Deserialize<TModel[]>(jsonData);

                if (models != null)
                {
                    modelList.AddRange(models);
                }
            }
        }
        #endregion Load and save

        #region Dispose
        public void Dispose()
        {
        }
        #endregion Dispose
    }
}

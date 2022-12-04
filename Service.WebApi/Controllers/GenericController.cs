using Microsoft.AspNetCore.Mvc;

namespace Service.WebApi.Controllers
{
    /// <summary>
    /// A generic one for the standard CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <typeparam name="TEditModel">The type of edit model</typeparam>
    /// <typeparam name="TModel">The type of model</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract partial class GenericController<TEditModel, TModel> : ControllerBase, IDisposable
        where TEditModel : class, new()
        where TModel : Logic.Contracts.IIdentifyable, new()
    {
        private bool disposedValue;

        protected Logic.GenericRepository<TModel> Repository { get; init; }
        protected GenericController()
        {
            Repository = new Logic.GenericRepository<TModel>($"{AppContext.BaseDirectory}\\Data\\{typeof(TModel).Name}.json");
        }
        /// <summary>
        /// Gets a list of models
        /// </summary>
        /// <returns>List of models</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> GetAsync()
        {
            var models = await Repository.GetAllAsync();

            return Ok(models);
        }

        /// <summary>
        /// Get a single model by Id.
        /// </summary>
        /// <param name="id">Id of the model to get</param>
        /// <response code="200">Model found</response>
        /// <response code="404">Model not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel?>> GetAsync(int id)
        {
            var model = await Repository.GetByIdAsync(id);

            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Adds a model.
        /// </summary>
        /// <param name="model">Model to add</param>
        /// <returns>Data about the created model (including primary key)</returns>
        /// <response code="201">Model created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public virtual async Task<ActionResult<TModel>> PostAsync([FromBody] TEditModel editModel)
        {
            var model = new TModel();

            model.CopyFrom(editModel);
            await Repository.InsertAsync(model);
            await Repository.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = model.Id }, model);
        }

        /// <summary>
        /// Updates a model
        /// </summary>
        /// <param name="id">Id of the model to update</param>
        /// <param name="editModel">Data to update</param>
        /// <returns>Data about the updated model</returns>
        /// <response code="200">Model updated</response>
        /// <response code="404">Model not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TModel>> PutAsync(int id, [FromBody] TEditModel editModel)
        {
            var model = await Repository.GetByIdAsync(id);

            if (model != null)
            {
                model.CopyFrom(editModel);
                await Repository.UpdateAsync(model);
                await Repository.SaveChangesAsync();
            }
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Delete a single model by Id
        /// </summary>
        /// <param name="id">Id of the model to delete</param>
        /// <response code="204">Model deleted</response>
        /// <response code="404">Model not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> DeleteAsync(int id)
        {
            var entity = await Repository.GetByIdAsync(id);

            if (entity != null)
            {
                await Repository.DeleteAsync(entity.Id);
                await Repository.SaveChangesAsync();
            }
            return entity == null ? NotFound() : NoContent();
        }

        #region Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose pattern
    }
}

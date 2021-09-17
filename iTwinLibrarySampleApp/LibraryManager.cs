/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using ItwinLibrarySampleApp.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ItwinLibrarySampleApp
    {
    internal class LibraryManager : IAsyncDisposable
        {
        private EndpointManager _endpointMgr;
        private List<Catalog> _catalogs; // Catalogs that will be deleted in DisposeAsync
        private List<Component> _components; // Components that will be deleted in DisposeAsync
        private List<Category> _categories; // Catalogs that will be deleted in DisposeAsync
        private List<Application> _applications; // Catalogs that will be deleted in DisposeAsync
        private List<Manufacturer> _manufacturers; // Catalogs that will be deleted in DisposeAsync


        #region Constructors
        internal LibraryManager (string token)
            {
            _endpointMgr = new EndpointManager(token);
            _catalogs = new List<Catalog>();
            _components = new List<Component>();
            _categories = new List<Category>();
            _applications = new List<Application>();
            _manufacturers = new List<Manufacturer>();
            }

        public async ValueTask DisposeAsync ()
            {
            Console.Write($"\n\n Deleting any components that were created");
            foreach ( var c in _components )
                await DeleteComponent(c.Id);

            Console.Write($"\n\n Deleting any catalogs that were created");
            foreach ( var c in _catalogs )
                await DeleteCatalog(c.Id);

            Console.Write($"\n\n Deleting any categories that were created");
            foreach ( var c in _categories )
                await DeleteCategory(c.Id);

            Console.Write($"\n\n Deleting any applications that were created");
            foreach ( var a in _applications )
                await DeleteApplication(a.Id);

            Console.Write($"\n\n Deleting any manufacturers that were created");
            foreach ( var m in _manufacturers )
                await DeleteManufacturer(m.Id);

            Console.Write("\n\n Clean Up Complete \n\n");
            }

        #endregion

        #region GET

        /// <summary> 
        /// Get components - This will return components that the user can access. It is not using paging so it will only return
        /// the top 100 catalogs by default. $skip and $top can be used to get the result in pages.
        /// </summary>
        /// <param name="catalogs"></param>
        /// 
        /// Filter by catalogs. it expects a comma separated list of catalogs
        /// This is the SQL equivalent of catalog in [catalogId1, catalogId2]
        /// 
        /// <param name="search"></param>
        /// 
        /// Get components - Wildcard Search. It should return any components with "iTwinSample" in the hashTags or displayName.
        /// This is the SQL equivalent of ( displayName like '%iTwinSample%')
        /// If 'iTwin Sample' is used in stead of 'iTwinSample' - result will also contain entites having only 'iTwin' or 'Sample' in displayName or hashtags as well.
        /// <returns></returns>
        internal async Task<List<Component>> GetComponents (string catalogs = null, string search = null)
            {
            var showAllPropertiesHeader = new Dictionary<string, string>
                {
                    { "Prefer", "return=representation" }
                };

            string queryString = string.Empty;
            if ( !string.IsNullOrWhiteSpace(catalogs) )
                {
                Console.Write($"\n\n Getting List of Components with catalogs={catalogs}");
                queryString = $"?catalogs={catalogs}";
                }
            if ( !string.IsNullOrWhiteSpace(search) )
                {
                Console.Write($"\n\n Getting List of Components with $search={search}");
                queryString = $"?$search={search}";
                }
            else
                Console.Write("\n\n- Getting List of Components");

            var responseMsg = await _endpointMgr.MakeGetCall<Component>($"/library/components{queryString}", showAllPropertiesHeader);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" [Retrieved {responseMsg.Instances?.Count} Components] (SUCCESS)");

            return responseMsg.Instances;
            }

        /// <summary> 
        /// Get catalogs - This will return catalogs that the user can access.  It is not using paging so it will only return
        /// the top 100 catalogs by default. $skip and $top can be used to get the result in pages.
        /// </summary>
        /// <param name="search"></param>
        /// 
        /// Get catalogs - Wildcard Search. It should return any catalog with "iTwin Sample" in the hashTags or displayName.
        /// This is the SQL equivalent of (displayName like '%iTwin Sample%')
        /// 
        /// <returns></returns>
        internal async Task<List<Catalog>> GetCatalogs (string search = null)
            {
            var showAllPropertiesHeader = new Dictionary<string, string>
                {
                    { "Prefer", "return=representation" }
                };

            string queryString = string.Empty;
            if ( !string.IsNullOrWhiteSpace(search) )
                {
                Console.Write($"\n\n Getting List of Catalogs with $search={search}");
                queryString = $"?$search={search}";
                }
            else
                Console.Write("\n\n Getting List of Catalogs");

            var responseMsg = await _endpointMgr.MakeGetCall<Catalog>($"/library/catalogs{queryString}", showAllPropertiesHeader);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" [Retrieved {responseMsg.Instances?.Count} Catalogs] (SUCCESS)");

            return responseMsg.Instances;
            }

        /// <summary>
        /// Get single catalog using the specified catalog id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<Catalog> GetCatalog (string id)
            {
            Console.Write($"\n\n Getting Catalog with id {id}");

            var showAllPropertiesHeader = new Dictionary<string, string>
                {
                    { "Prefer", "return=representation" }
                };
            var responseMsg = await _endpointMgr.MakeGetSingleCall<Catalog>($"/library/catalogs/{id}", showAllPropertiesHeader);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            return responseMsg.Instance;
            }

        /// <summary>
        /// Get single component using the specified component id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<Component> GetComponent (string url)
            {
            Console.Write($"\n\n Getting Component with url {url}");

            var responseMsg = await _endpointMgr.MakeGetSingleCall<Component>(url);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            return responseMsg.Instance;
            }

        /// <summary> 
        /// Get all the documents for the specified component id.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<List<Document>> GetComponentDocuments (string componentId)
            {

            Console.Write($"\n\n Getting List of Documents for Component Id '{componentId}'");

            var responseMsg = await _endpointMgr.MakeGetCall<Document>($"/library/components/{componentId}/documents");
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" [Retrieved {responseMsg.Instances?.Count} Documents] (SUCCESS)");

            return responseMsg.Instances;
            }

        /// <summary> 
        /// Get all the variations for the specified component id.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<List<Variation>> GetComponentVariations (string componentId)
            {

            Console.Write($"\n\n Getting List of Variations for Component Id '{componentId}'");

            var responseMsg = await _endpointMgr.MakeGetCall<Variation>($"/library/components/{componentId}/variations");
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" [Retrieved {responseMsg.Instances?.Count} Variations] (SUCCESS)");

            return responseMsg.Instances;
            }

        /// <summary> 
        /// Get all the webLinks for the specified component id.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<List<WebLink>> GetComponentWebLinks (string componentId)
            {

            Console.Write($"\n\n Getting List of WebLinks for Component Id '{componentId}'");

            var responseMsg = await _endpointMgr.MakeGetCall<WebLink>($"/library/components/{componentId}/weblinks");
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" [Retrieved {responseMsg.Instances?.Count} WebLinks] (SUCCESS)");

            return responseMsg.Instances;
            }

        #endregion

        #region POST
        /// <summary>
        /// Create catalog (POST)
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="cleanupLater"></param>
        /// <returns></returns>
        internal async Task<Catalog> CreateCatalog (Catalog catalog = null, bool cleanupLater = true)
            {
            if ( catalog == null )
                catalog = new Catalog();

            Console.Write("\n\n Creating a Catalog");

            var responseMsg = await _endpointMgr.MakePostCall("/library/catalogs", catalog);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            if ( cleanupLater )
                _catalogs.Add(responseMsg.NewInstance);

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create component (POST)
        /// </summary>
        /// <param name="component"></param>
        /// <param name="cleanupLater"></param>
        /// <returns></returns>
        internal async Task<Component> CreateComponent (ComponentPost component = null, bool cleanupLater = true)
            {
            if ( component == null )
                component = new ComponentPost();

            Console.Write("\n\n Creating a Component");

            var responseMsg = await _endpointMgr.MakePostCall<ComponentPost, Component>("/library/components", component);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            if ( cleanupLater )
                _components.Add(responseMsg.NewInstance);

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create category (POST)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="cleanupLater"></param>
        /// <returns></returns>
        internal async Task<Category> CreateCategory (Category category = null, bool cleanupLater = true)
            {
            if ( category == null )
                category = new Category();

            Console.Write("\n\n Creating a Category");

            var responseMsg = await _endpointMgr.MakePostCall("/library/categories", category);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            if ( cleanupLater )
                _categories.Add(responseMsg.NewInstance);

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create application (POST)
        /// </summary>
        /// <param name="application"></param>
        /// <param name="cleanupLater"></param>
        /// <returns></returns>
        internal async Task<Application> CreateApplication (Application application = null, bool cleanupLater = true)
            {
            if ( application == null )
                application = new Application();

            Console.Write("\n\n Creating a Application");

            var responseMsg = await _endpointMgr.MakePostCall("/library/applications", application);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            if ( cleanupLater )
                _applications.Add(responseMsg.NewInstance);

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create manufacturer (POST)
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="cleanupLater"></param>
        /// <returns></returns>
        internal async Task<Manufacturer> CreateManufacturer (Manufacturer manufacturer = null, bool cleanupLater = true)
            {
            if ( manufacturer == null )
                manufacturer = new Manufacturer();

            Console.Write("\n\n Creating a Manufacturer");

            var responseMsg = await _endpointMgr.MakePostCall("/library/manufacturers", manufacturer);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            if ( cleanupLater )
                _manufacturers.Add(responseMsg.NewInstance);

            return responseMsg.NewInstance;
            }
        /// <summary>
        /// Create document (POST)
        /// </summary>
        /// <param name="document"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<Document> CreateComponentDocument (DocumentPost document, string componentId)
            {

            Console.Write("\n\n Creating a Document");

            var responseMsg = await _endpointMgr.MakePostCall<DocumentPost, Document>($"/library/components/{componentId}/documents", document);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create variation (POST)
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<Variation> CreateComponentVariation (Variation variation, string componentId)
            {

            Console.Write("\n\n Creating a Variation");

            var responseMsg = await _endpointMgr.MakePostCall($"/library/components/{componentId}/variations", variation);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write(" (SUCCESS)");

            return responseMsg.NewInstance;
            }

        /// <summary>
        /// Create webLink (POST)
        /// </summary>
        /// <param name="webLink"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<WebLink> CreateComponentWebLink (WebLink webLink, string componentId)
            {

            Console.Write("\n\n Creating a WebLink");

            var responseMsg = await _endpointMgr.MakePostCall($"/library/components/{componentId}/weblinks", webLink);
            if ( responseMsg.Status != HttpStatusCode.Created )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" (SUCCESS)");

            return responseMsg.NewInstance;
            }

        #endregion

        #region PUT
        /// <summary>
        /// Update catalog (PUT)
        /// You need to specify all the desired properties of the entity, since this will replace existing entity definition with the current one.
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task UpdateCatalog (Catalog catalog, string id)
            {
            Console.Write($"\n\n Updating Catalog for ({id})");

            var responseMsg = await _endpointMgr.MakePutCall<Catalog>($"/library/catalogs/{id}", catalog);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" (SUCCESS)");
            }

        /// <summary>
        /// Update component (PUT)
        /// You need to specify all the desired properties of the entity, since this will replace existing entity definition with the current one.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task UpdateComponent (ComponentPost component, string id)
            {
            Console.Write($"\n\n Updating Component for ({id})");

            var responseMsg = await _endpointMgr.MakePutCall<ComponentPost, Component>($"/library/components/{id}", component);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" (SUCCESS)");
            }

        /// <summary>
        /// Update document (PUT)
        /// You need to specify all the desired properties of the entity, since this will replace existing entity definition with the current one.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="id"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        internal async Task<Document> UpdateComponentDocument (DocumentPost document, string id, string componentId)
            {
            Console.Write($"\n\n Updating Document for ({id})");

            var responseMsg = await _endpointMgr.MakePutCall<DocumentPost, Document>($"/library/components/{componentId}/documents/{id}", document);
            if ( responseMsg.Status != HttpStatusCode.OK )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");

            Console.Write($" (SUCCESS)");
            return responseMsg.UpdatedInstance;
            }

        #endregion

        #region DELETE
        /// <summary>
        /// Delete catalog (DELETE) - If you are writing tests for your code, always delete catalogs when they are no longer needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteCatalog (string id)
            {
            var responseMsg = await _endpointMgr.MakeDeleteCall<Catalog>($"/library/catalogs/{id}");
            if ( responseMsg.Status != HttpStatusCode.NoContent )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");
            }

        /// <summary>
        /// Delete component (DELETE) - If you are writing tests for your code, always delete components when they are no longer needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteComponent (string id)
            {
            var responseMsg = await _endpointMgr.MakeDeleteCall<Component>($"/library/components/{id}");
            if ( responseMsg.Status != HttpStatusCode.NoContent )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");
            }

        /// <summary>
        /// Delete category (DELETE) - If you are writing tests for your code, always delete categories when they are no longer needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteCategory (string id)
            {
            var responseMsg = await _endpointMgr.MakeDeleteCall<Category>($"/library/categories/{id}");
            if ( responseMsg.Status != HttpStatusCode.NoContent )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");
            }

        /// <summary>
        /// Delete application (DELETE) - If you are writing tests for your code, always delete applications when they are no longer needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteApplication (string id)
            {
            var responseMsg = await _endpointMgr.MakeDeleteCall<Application>($"/library/applications/{id}");
            if ( responseMsg.Status != HttpStatusCode.NoContent )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");
            }

        /// <summary>
        /// Delete manufacturer (DELETE) - If you are writing tests for your code, always delete manufacturers when they are no longer needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteManufacturer (string id)
            {
            var responseMsg = await _endpointMgr.MakeDeleteCall<Manufacturer>($"/library/manufacturers/{id}");
            if ( responseMsg.Status != HttpStatusCode.NoContent )
                throw new Exception($"{responseMsg.Status}: {responseMsg.ErrorDetails?.Code} - {responseMsg.ErrorDetails?.Message}");
            }
        #endregion

        #region Others

        /// <summary>
        /// Adds a new document to provided component Id
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="purpose"></param>
        /// <param name="ext"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal async Task<Document> AddComponentDocument (string componentId, Purpose purpose, string ext, string filePath)
            {
            Console.Write($"\n\n Adding '{purpose}' Document to Component '{componentId}'");

            var docToCreate = new DocumentPost(purpose, ext);
            var document = await CreateComponentDocument(docToCreate, componentId);
            await _endpointMgr.MakeFileUploadCall(document.Links.FileUrl.Href, filePath);
            var docToUpdate = document.ConvertToDocumentPost();
            docToUpdate.Available = true;
            var updateDocument = await UpdateComponentDocument(docToUpdate, document.Id, componentId);

            Console.Write($"\n\n Adding '{purpose}' Document to Component '{componentId}' SUCESS");
            return updateDocument;
            }

        internal async Task<Variation> AddComponentVariation (string componentId, string documentId)
            {
            var varToCreate = new Variation(documentId);
            //Sample adhoc properties - there is no limit on number of adhoc properties.
            varToCreate.AdHocProperties.Add(new AdHocProperty("Height", PropertyType.IntegerType, "1000", "Milimeters"));
            varToCreate.AdHocProperties.Add(new AdHocProperty("Width", PropertyType.IntegerType, "50", "Milimeters"));
            varToCreate.AdHocProperties.Add(new AdHocProperty("Length", PropertyType.IntegerType, "500", "Milimeters"));
            varToCreate.AdHocProperties.Add(new AdHocProperty("Material", PropertyType.StringType, "Wood", null));
            var variation = await CreateComponentVariation(varToCreate, componentId);
            return variation;
            }

        #endregion

        #region WorkFlows
        /// <summary>
        /// Demonstrates create component functionality.
        /// </summary>
        /// <returns></returns>
        internal async Task CreateComponentWorkflow (bool cleanupLater = true)
            {
            Console.Write($"\n\n Create Component Workflow");

            //Create a new catalog to upload components in it
            var catalog = await CreateCatalog(null, cleanupLater);
            //Create a new category to add as a referenced entity to component or alternativley get an existing one.
            var category = await CreateCategory(null, cleanupLater);
            //Create a new application to add as a referenced entity to component or alternativley get an existing one.
            var application = await CreateApplication(null, cleanupLater);
            //Create a new manufacturer to add as a referenced entity to component or alternativley get an existing one.
            var manufacturer = await CreateManufacturer(null, cleanupLater);

            // Create component post body by providing appropriate referenced entities (catalogs, category, application, manufacturer)
            var componentToCreate = new ComponentPost(new List<string> { catalog.Id }, category.Id, application.Id, manufacturer.Id);
            var component = await CreateComponent(componentToCreate, cleanupLater);

            //Add documents
            //For the purpose of this sample, it is assumed files required to create documents are placed at the executing 
            //assemply location - this path or file names can be changed accordingly.
            //Add design document to component
            Console.Write($"\n\n Associate documents to component");
            var designFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\iTwinSample.rfa";
            var designDocument = await AddComponentDocument(component.Id, Purpose.Design, "rfa", designFilePath);
            //Add thumbnail document to component
            var thumbnailFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\iTwinSample.png";
            var thumbnailDocument = await AddComponentDocument(component.Id, Purpose.Thumbnail, "png", thumbnailFilePath);
            //Add reference document to component
            var referenceFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\iTwinSample.pdf";
            var referenceDocument = await AddComponentDocument(component.Id, Purpose.Reference, "pdf", referenceFilePath);
            Console.Write($"\n Associate documents to component - (SUCCESS)");

            //Add variation
            var variation = await AddComponentVariation(component.Id, designDocument.Id);
            //Add webLink
            var webLink = await CreateComponentWebLink(new WebLink(), component.Id);

            //Get all documents from the component 
            var documents = await GetComponentDocuments(component.Id);
            //Get all variations from the component
            var variations = await GetComponentVariations(component.Id);
            //Get all webLinks from the component
            var webLinks = await GetComponentWebLinks(component.Id);

            Console.Write($"\n\n Create Component Workflow (SUCCESS)");
            }

        /// <summary>
        /// Demonstrates upload component functionality.
        /// </summary>
        /// <returns></returns>
        internal async Task UploadComponentToCatalogWorkflow ()
            {
            Console.Write($"\n\n Upload Component to Catalog Workflow");
            //Get an existing catalog having 'iTwinSample' in displayName or in hashtags.
            var catalog = (await GetCatalogs("iTwinSample")).FirstOrDefault();

            if ( null == catalog )
                {
                //Create a new catalog to upload components in it
                catalog = await CreateCatalog(null, false);
                }

            // Make upload request body - set the design file name and the catalog Id. (Make sure you have a design file *.rfa to upload)
            // a dgn or cel file can also be uploaded with same steps - change the file name accordingly.
            var uploadPayload = new UploadComponentPost("iTwinSample.rfa", new List<string> { catalog.Id });

            Console.Write($"\n\n Make Upload Request");
            // Make upload request
            var uploadResult = await _endpointMgr.MakePostCall<UploadComponentPost, UploadComponent>("/library/upload", uploadPayload);
            Console.Write($" (SUCCESS)");

            //Upload design file - assuming a file named 'sample.rfa' exists at the location, this code is being executed.
            //Change the location or file name according to your needs.
            var localFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\iTwinSample.rfa";
            await _endpointMgr.MakeFileUploadCall(uploadResult.NewInstance.Links.DesignFileUploadUrl.Href, localFilePath);

            // Upload component is a long running activity and runs in background, It starts once design file upload is complete.
            // Response of upload request contains location header containing jobs endpoint with current upload job id, this is used
            // to track the status of the background upload activity.
            var jobResponse = await _endpointMgr.GetJobAfterCompletion(uploadResult.Headers.Location?.ToString());
            Console.Write($"\n\n Upload Component Completed");

            // After upload job is complete, job response contains url for the uploaded component - make the GET
            //request to this url to get component
            var component = await GetComponent(jobResponse.Instance.Links.Component.Href);
            var componentPost = component.ConvertToComponentPost();
            //Make changes in the component and make an update request
            componentPost.State = ComponentState.Approved;
            await UpdateComponent(componentPost, component.Id);

            //Add a webLink to the component
            var webLink = await CreateComponentWebLink(new WebLink(), component.Id);

            Console.Write($"\n\n Query Components from Catalog");
            //Query components
            //Get all the components from catalog having 'iTwinSample' in the displayName or hashTags. there should be atleast
            //one we have just uploaded.
            var components = await GetComponents(catalog.Id, "iTwinSample");
            component = components.FirstOrDefault();

            if (null == component )
                {
                Console.Write($"\n\n No Component Found from Catalog '{catalog.Id}'");
                return;
                }

            Console.Write($"\n\n Get Related Entites for Component Id '{component.Id}'");
            //Get all the documents of the component.
            var documents = await GetComponentDocuments(component.Id);
            var designDocument = documents.Where(x => x.Purpose == Purpose.Design).FirstOrDefault();
            //Download design file associated to design document of the component - path to download can be changed according to needs.
            var pathToDownload = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{designDocument.DisplayName}.{designDocument.Extension}";
            await _endpointMgr.MakeFileDownloadCall(designDocument.Links.FileUrl.Href, pathToDownload);

            //Get all the variations from a component.
            var variations = await GetComponentVariations(component.Id);

            //Get all the webLinks from a component.
            var webLinks = await GetComponentWebLinks(component.Id);
            }

        #endregion

        }
    }

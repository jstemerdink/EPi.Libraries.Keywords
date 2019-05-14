Add the [KeywordsMetaTag] to your keywords property.

The service you want to use needs to be injected. You can use either the Azure provider I created in EPi.Libraries.Keywords.Azure 
Or write your own for the service you would like to use. In that case you will need to implement IExtractionService 
and add the following attribute to your class [ServiceConfiguration(typeof(IExtractionService))]
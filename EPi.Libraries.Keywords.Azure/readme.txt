Add the [KeywordsMetaTag] to your keywords property.

The following keys need to be  added to your appsettings.json: 

"Azure": {
  "TextAnalytics": {
    "Key": "changeme.api-key",
    "Endpoint": "https://westeurope.api.cognitive.microsoft.com"
  }
}

Note that not all languages are supported by Azure (https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/language-support)
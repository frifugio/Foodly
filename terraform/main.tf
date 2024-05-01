resource "azurerm_resource_group" "foodly-rg" {
  location   = "westeurope"
  managed_by = null
  name       = "foodly-rg"
  tags       = {}
}

resource "azurerm_cosmosdb_sql_database" "foodly-db" {
  account_name        = azurerm_cosmosdb_account.foodly-cosmos-account.name
  name                = "foodly-db"
  resource_group_name = azurerm_resource_group.foodly-rg.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "foodly-db-foods-container" {
  account_name           = azurerm_cosmosdb_account.foodly-cosmos-account.name
  analytical_storage_ttl = null
  database_name          = azurerm_cosmosdb_sql_database.foodly-db.name
  default_ttl            = null
  name                   = "foods"
  partition_key_path     = "/name"
  partition_key_version  = null
  resource_group_name    = azurerm_resource_group.foodly-rg.name
  conflict_resolution_policy {
    conflict_resolution_path      = "/_ts"
    conflict_resolution_procedure = null
    mode                          = "LastWriterWins"
  }
  indexing_policy {
    indexing_mode = "consistent"
    included_path {
      path = "/*"
    }
  }
}

resource "azurerm_cosmosdb_account" "foodly-cosmos-account" {
  access_key_metadata_writes_enabled    = true
  analytical_storage_enabled            = false
  automatic_failover_enabled            = false
  create_mode                           = null
  default_identity_type                 = "FirstPartyIdentity"
  free_tier_enabled                     = true
  ip_range_filter                       = null
  is_virtual_network_filter_enabled     = false
  key_vault_key_id                      = null
  kind                                  = "GlobalDocumentDB"
  local_authentication_disabled         = false
  location                              = "westeurope"
  minimal_tls_version                   = "Tls12"
  mongo_server_version                  = null
  multiple_write_locations_enabled      = false
  name                                  = "foodly-cosmos"
  network_acl_bypass_for_azure_services = false
  network_acl_bypass_ids                = []
  offer_type                            = "Standard"
  partition_merge_enabled               = false
  public_network_access_enabled         = true
  resource_group_name                   = azurerm_resource_group.foodly-rg.name
  tags = {
    defaultExperience       = "Core (SQL)"
    hidden-cosmos-mmspecial = ""
  }
  analytical_storage {
    schema_type = "WellDefined"
  }
  backup {
    interval_in_minutes = 240
    retention_in_hours  = 8
    storage_redundancy  = "Local"
    tier                = null
    type                = "Periodic"
  }
  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }
  geo_location {
    failover_priority = 0
    location          = "westeurope"
    zone_redundant    = false
  }
}

resource "azurerm_application_insights" "foodly-appinsight" {
  application_type                      = "web"
  daily_data_cap_in_gb                  = 100
  daily_data_cap_notifications_disabled = false
  disable_ip_masking                    = false
  force_customer_storage_for_profiler   = false
  internet_ingestion_enabled            = true
  internet_query_enabled                = true
  local_authentication_disabled         = false
  location                              = "westeurope"
  name                                  = "foodly"
  resource_group_name                   = azurerm_resource_group.foodly-rg.name
  retention_in_days                     = 90
  sampling_percentage                   = 0
  tags                                  = {}
  workspace_id                          = "/subscriptions/e104f70f-e551-411f-9322-17239bd6bf38/resourceGroups/defaultresourcegroup-weu/providers/Microsoft.OperationalInsights/workspaces/defaultworkspace-e104f70f-e551-411f-9322-17239bd6bf38-weu"
}

resource "azurerm_static_web_app" "foodly" {
  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.foodly-appinsight.connection_string
    CosmosDBConnectionString              = azurerm_cosmosdb_account.foodly-cosmos-account.primary_sql_connection_string
    MyContainer                           = azurerm_cosmosdb_sql_container.foodly-db-foods-container.name
    MyCosmosAccount                       = azurerm_cosmosdb_account.foodly-cosmos-account.name
    MyDatabase                            = azurerm_cosmosdb_sql_database.foodly-db.name
  }
  configuration_file_changes_enabled = true
  location                           = "westeurope"
  name                               = "foodly"
  preview_environments_enabled       = true
  resource_group_name                = azurerm_resource_group.foodly-rg.name
  sku_size                           = "Free"
  sku_tier                           = "Free"
  tags = {
    "hidden-link: /app-insights-conn-string"         = azurerm_application_insights.foodly-appinsight.connection_string
    "hidden-link: /app-insights-instrumentation-key" = azurerm_application_insights.foodly-appinsight.instrumentation_key
    "hidden-link: /app-insights-resource-id"         = azurerm_application_insights.foodly-appinsight.id
  }
}
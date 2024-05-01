terraform {
    required_providers {
        azurerm = {
            source = "hashicorp/azurerm"
            version = "~>3.101"
        }
    }
    backend "azurerm" {
      resource_group_name = "foodly-rg"
      storage_account_name = "foodlytfstate"
      container_name = "tfstate"
      key = "terraform.tfstate"
    }
}

provider "azurerm" {
  features {}

  client_id       = var.azure-client-id
  client_secret   = var.azure-client-secret
  tenant_id       = var.azure-tenant-id
  subscription_id = var.azure-subscription-id
}
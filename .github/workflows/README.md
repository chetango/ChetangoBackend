# GitHub Actions Workflows

Este directorio contiene los workflows de CI/CD para el proyecto Chetango Backend.

## azure-deploy-production.yml

Workflow de deployment automático a Azure App Service.

**Trigger:** Push a la rama `develop`

**Destino:** 
- App Service: `chetango-api-prod`
- URL: https://api.corporacionchetango.com

**Proceso:**
1. Checkout del código
2. Setup .NET 9
3. Build del proyecto
4. Publish
5. Deploy a Azure usando publish profile

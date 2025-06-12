# Customer-Api

## Requirement 1: Build an API
- Built using **C#**, **EF Core**, and **In-Memory Database**
- Accessible via Swagger: `http://localhost:XXX/swagger/index.html`
- [Api Code](https://github.com/yashshah7575/customer-api/tree/main/src)
### 🔹 Endpoints

#### Customers
- `GET /api/customers` — Get all customers  
- `GET /api/customers/{id}` — Get customer by ID  
- `POST /api/customers` — Create new customer  
- `PUT /api/customers/{id}` — Update customer  
- `DELETE /api/customers/{id}` — Delete customer by ID  

#### System
- `GET /api/system/ping` — Health check  
- `GET /api/system/version` — API version  

---

## Requirement 2: Integration and/or Acceptance Testing

- Integration tests available in the below project: [Tests Code](https://github.com/yashshah7575/customer-api/tree/main/src/Customer.Api.Integration.Tests)
  ```bash
  Customer.Api.Integration.Tests
   ```

---

## Requirement 3 : Observability
- Logging is enabled in the Repository Layer as a sample.
- Application metrics tracking can be implemented using OpenTelemetry.
- We can enable UI as well for such metrics using nuget pakcage AspNetCore.HealthChecks.UI

---

## Requirement - 4: Containerization
[Docker Code](https://github.com/yashshah7575/customer-api/blob/main/src/Customer.Api/Dockerfile)

### Steps to build docker image:
1. Run docker desktop
2. Open "Customer.Api/Customer.Api/" folder in terminal
3. Run command "docker buildx build ."

### Steps to run image
Now you have image build, you can simply run

  ```bash
  docker run -p 8080:80 customer-api
   ```

---

## Requirement - 5: Kubernetes
I am aware of Kubernetes concepts. But, I haven't worked on this. So skipping this section

## Requirement - 6: CI/CD
- [IAC Code](https://github.com/yashshah7575/customer-api/blob/main/src/main.tf)
- Steps in pipeline:
    1. Checkout code from GitHub
    2. Build
    3. Run unit tests
    4. Deploy to Dev
    5. Run automation tests (Dev)
    6. Deploy to Staging (STG)
    7. Run automation tests (STG)
    8. Manual approval
    9. Deploy to UAT
    10. Run automation tests (UAT)
    11. Manual approval
    12. Deploy to Production (PROD)
    13. Run automation tests (PROD)

## Step 7: App Integration
A sample console client app is available at:
[Integration Console App](https://github.com/yashshah7575/customer-api/tree/main/client/Customer.Client/Customer.Api.Client)
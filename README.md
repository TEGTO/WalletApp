### Installation Guide

1. **Clone the repository**:
    ```bash
    git clone https://github.com/TEGTO/WalletApp.git
    ```
    
2. **Navigate to the project directory and open the solution file**:
    ```bash
    cd src/WalletApp.Backend
    ```
    Open the `.sln` file in your preferred IDE (e.g., Visual Studio).
   
4. **Set up the environment configuration**:
    - Add a [.env](https://drive.google.com/file/d/1wAdyoMkACe_8rc7P9Ygwb6HDHNUtoDJk/view?usp=sharing) file or create one manually based on the required configuration.
       
5. **Run the project using Docker Compose or run docker-compose project via Visual Studio**:
    ```bash
    docker-compose up
    ```
    
6. **Access the Swagger documentation**:
    - Open your browser and go to: [http://localhost:7122/swagger/index.html](http://localhost:7122/swagger/index.html).

---

### Quick Start Guide

1. **Register a new user**:
    Use the following payload in the registration endpoint:
    ```json
    {
      "email": "example@gmail.com",
      "name": "John",
      "login": "example",
      "password": "123456qwerty",
      "confirmPassword": "123456qwerty"
    }
    ```
    
2. **Authorize in Swagger**:
    - Add your access token to the Swagger UI for authentication.
      
3. **Explore and test endpoints**:
    - Use available endpoints in Swagger for testing the application features.

**Note**: To make a transaction, you must add a card first.

# Askly - AI-Powered Chatbot Application

A modern, full-stack AI chatbot application built with React, .NET 8, and AWS infrastructure. This project demonstrates advanced cloud-native development patterns, AI integration, and serverless architecture.

## 🌐 Live Demo

Try the application live:

- **Production Environment:** https://askly.microapps.info/
- **Development Environment:** https://dev-askly.microapps.info/

## 🔗 About Me

- **Author:** Alexandros Kavroulakis
- **LinkedIn:** https://www.linkedin.com/in/alexandros-kavroulakis/

---

## 🚀 Features

- **AI-Powered Chatbot** using GitHub's AI models (GPT-4o-mini)
- **Web Search Integration** via Tavily API for real-time information retrieval
- **LinkedIn Profile Enrichment** using Scrapin API
- **Google OAuth Authentication** for secure user login
- **Serverless Architecture** deployed on AWS Lambda
- **CloudFront CDN** for fast global content delivery
- **PostgreSQL Database** with AWS Aurora DSQL integration
- **Continuous Deployment** via GitHub Actions
- **Infrastructure as Code** using Terraform

## 🛠️ Tech Stack

### Frontend

- **React 19** with TypeScript
- **React Router** for navigation
- **Google OAuth** for authentication
- **Modern CSS** with responsive design

### Backend

- **.NET 8** with ASP.NET Core
- **AWS Lambda** for serverless hosting
- **Entity Framework Core** with PostgreSQL
- **Microsoft Agents AI** framework
- **JWT Authentication**

### Infrastructure

- **AWS Lambda** - Serverless API hosting
- **AWS S3** - Static website hosting
- **AWS CloudFront** - Content delivery network
- **AWS Aurora DSQL** - PostgreSQL database
- **AWS Systems Manager** - Secret management
- **Terraform** - Infrastructure as Code

### AI & APIs

- **GitHub AI Models** - GPT-4o-mini for chatbot logic
- **Tavily API** - Web search capabilities
- **Scrapin API** - LinkedIn profile enrichment

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [Node.js](https://nodejs.org/) (v18 or higher)
- [.NET SDK](https://dotnet.microsoft.com/) (8.0 or higher)
- [Terraform](https://www.terraform.io/) (latest version)
- [AWS CLI](https://aws.amazon.com/cli/) configured with credentials
- [Git](https://git-scm.com/)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/chatbot-v2.git
cd chatbot-v2
```

### 2. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
cp .env.example .env.local

# Edit .env.local with your configuration
# REACT_APP_API_URL=http://localhost:54793
# REACT_APP_GOOGLE_AUTH_CLIENT_ID=your-google-client-id

# Start development server
npm start
```

### 3. Backend Setup

```bash
cd backend/src/lambda-api

# Install dependencies
dotnet restore

# Set up environment variables (create .env file)
# REGION=eu-west-2
# CLUSTER_USER=admin
# CLUSTER_ENDPOINT=your-endpoint

# Run the application
dotnet run
```

### 4. Infrastructure Setup

```bash
cd infrastructure

# Initialize Terraform
terraform init

# Plan infrastructure changes
terraform plan

# Apply infrastructure (requires AWS credentials)
terraform apply
```

## 🔐 Environment Variables

### Frontend (.env.local)

```
REACT_APP_API_URL=your-api-url
REACT_APP_GOOGLE_AUTH_CLIENT_ID=your-google-client-id
```

### Backend (AWS Systems Manager Parameter Store)

```
TavilyAPIKey=your-tavily-api-key
ScrapintAPIKey=your-scrapin-api-key
GitHubModelsToken=your-github-models-token
Jwt={...}
Google={...}
```

## 🧪 Testing

```bash
# Frontend tests
cd frontend
npm test

# Backend tests
cd backend/src/lambda-api
dotnet test
```

## 📦 Deployment

The project uses GitHub Actions for continuous deployment:

- **Main branch** → Deploys to development environment
- **Production branch** → Deploys to production environment

Deployment includes:

1. Building and testing the frontend
2. Deploying frontend to S3
3. Building and packaging the Lambda function
4. Deploying Lambda function to AWS
5. Running database migrations

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- [React](https://reactjs.org/)
- [.NET](https://dotnet.microsoft.com/)
- [AWS](https://aws.amazon.com/)
- [GitHub AI](https://github.com/features/ai)
- [Tavily](https://tavily.com/)
- [Scrapin](https://scrapin.io/)
- [Udemy LangChain Course](https://www.udemy.com/course/langchain/learn/lecture/44651779#overview)

**Note:** Remember to add your `.env` files to `.gitignore` and never commit sensitive information like API keys or secrets to version control.

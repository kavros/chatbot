# Askly - AI-Powered Chatbot Application

This is a chatbot driven by an autonomous AI agent that can search the web in real time to answer your questions.

## 🌐 Live Demo

Try the application live:

- **Production Environment:** https://askly.microapps.info/
- **Development Environment:** https://dev-askly.microapps.info/

## 🚀 Features

- **AI-Powered Chatbot** using GitHub's AI models (GPT-4o-mini)
- **Web Search Integration** via Tavily API for real-time information retrieval
- **Google OAuth Authentication** for secure user login
- **Serverless Architecture** deployed on AWS Lambda
- **CloudFront CDN** for fast global content delivery
- **PostgreSQL Database** with AWS Aurora DSQL integration
- **Continuous Deployment** via GitHub Actions
- **Infrastructure as Code** using Terraform

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

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [Node.js](https://nodejs.org/) (v18 or higher)
- [.NET SDK](https://dotnet.microsoft.com/) (8.0 or higher)
- [Terraform](https://www.terraform.io/) (latest version)
- [AWS CLI](https://aws.amazon.com/cli/) configured with credentials
- [Git](https://git-scm.com/)

## 🔐 Environment Variables

### Frontend (.env.local)

```
REACT_APP_API_URL=your-api-url
REACT_APP_GOOGLE_AUTH_CLIENT_ID=your-google-client-id
```

### Backend (AWS Systems Manager Parameter Store)

```
TavilyAPIKey=your-tavily-api-key
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
- [Udemy LangChain Course](https://www.udemy.com/course/langchain/learn/lecture/44651779#overview)

## 🔗 About Me

- **Author:** Alexandros Kavroulakis
- **LinkedIn:** https://www.linkedin.com/in/alexandros-kavroulakis/

pipeline {

    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {

        stage('Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/lisaying83/taskmanager-api.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet restore TaskManager.sln'
                bat 'dotnet build TaskManager.sln --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test TaskManager.sln --configuration Release --no-build'
            }
        }

    }

    post {

        success {
            echo 'Pipeline completed successfully.'
        }

        failure {
            echo 'Pipeline failed. Please check the Jenkins console output.'
        }

    }
}
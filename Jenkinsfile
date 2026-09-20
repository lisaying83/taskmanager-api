pipeline {

    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {

        stage('Build') {
            steps {
                bat 'dotnet restore TaskManager.sln'

                bat '''
                dotnet build TaskManager.sln ^
                --configuration Release ^
                --no-restore
                '''

                bat '''
                dotnet publish src\\TaskManager.Api\\TaskManager.Api.csproj ^
                --configuration Release ^
                --no-build ^
                --output artifacts\\publish
                '''

                archiveArtifacts artifacts: 'artifacts/publish/**',
                                 fingerprint: true
            }
        }

        stage('Test') {
            steps {
                bat '''
                dotnet test TaskManager.sln ^
                --configuration Release ^
                --no-build
                '''
            }
        }

        stage('Deploy') {
            steps {
                withEnv(['PATH+DOCKER=C:\\Users\\Huili Ying\\AppData\\Local\\Programs\\DockerDesktop\\resources\\bin']) {

                    bat 'docker version'
                    bat 'docker compose version'

                    bat 'docker compose down'
                    bat 'docker compose up -d --build'
                    bat 'docker compose ps'

                    powershell '''
                    $maxAttempts = 12
                    $attempt = 1

                    while ($attempt -le $maxAttempts) {
                        try {
                            $response = Invoke-WebRequest `
                                -Uri "http://localhost:5000/health" `
                                -UseBasicParsing `
                                -TimeoutSec 5

                            if ($response.StatusCode -eq 200) {
                                Write-Host "Application deployed successfully and is healthy."
                                exit 0
                            }
                        }
                        catch {
                            Write-Host "Waiting for application... attempt $attempt"
                        }

                        Start-Sleep -Seconds 5
                        $attempt++
                    }

                    Write-Error "Deployment health check failed."
                    exit 1
                    '''
                }
            }
        }
    }

    post {
        success {
            echo 'Pipeline completed successfully.'
        }

        failure {
            echo 'Pipeline failed. Check the console output.'

            bat 'docker compose logs --tail=100 || exit /b 0'
        }
    }
}
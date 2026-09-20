pipeline {

    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'

        IMAGE_NAME = 'taskmanager-api'
        IMAGE_TAG = "build-${BUILD_NUMBER}"
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

        stage('Code Quality') {
            steps {
                withCredentials([
                    string(
                        credentialsId: 'sonar-token',
                        variable: 'SONAR_TOKEN'
                    )
                ]) {
                    bat '''
                    if not exist .sonar-tools\\dotnet-sonarscanner.exe (
                        dotnet tool install --tool-path .sonar-tools dotnet-sonarscanner
                    )

                    .sonar-tools\\dotnet-sonarscanner.exe begin ^
                        /k:"lisaying83_taskmanager-api" ^
                        /o:"lisaying83" ^
                        /d:sonar.token="%SONAR_TOKEN%" ^
                        /d:sonar.qualitygate.wait=true ^
                        /d:sonar.qualitygate.timeout=300

                    dotnet build TaskManager.sln ^
                        --configuration Release ^
                        --no-incremental

                    .sonar-tools\\dotnet-sonarscanner.exe end ^
                        /d:sonar.token="%SONAR_TOKEN%"
                    '''
                }
            }
        }

        stage('Security') {
            steps {

                echo 'Running Trivy security scan...'

                bat '''
                "C:\\Trivy\\trivy.exe" fs ^
                --scanners vuln,misconfig,secret ^
                --severity HIGH,CRITICAL ^
                --format table ^
                --output trivy-report.txt ^
                .
                '''

                bat 'type trivy-report.txt'

                archiveArtifacts artifacts: 'trivy-report.txt',
                                fingerprint: true
            }
        }

        stage('Deploy') {
            steps {
                withEnv(['PATH+DOCKER=C:\\Users\\Huili Ying\\AppData\\Local\\Programs\\DockerDesktop\\resources\\bin']) {

                    bat 'docker compose -p taskmanager-staging down'
                    bat 'docker compose -p taskmanager-staging up -d --build'
                    bat 'docker compose -p taskmanager-staging ps'

                    powershell '''
                    $maxAttempts = 5
                    $attempt = 1

                    while ($attempt -le $maxAttempts) {
                        try {
                            $response = Invoke-WebRequest `
                                -Uri "http://localhost:5000/health" `
                                -UseBasicParsing `
                                -TimeoutSec 5

                            if ($response.StatusCode -eq 200) {
                                Write-Host "Application deployed to Stage successfully and is healthy."
                                exit 0
                            }
                        }
                        catch {
                            Write-Host "Waiting for staging application... attempt $attempt"
                        }

                        Start-Sleep -Seconds 5
                        $attempt++
                    }

                    Write-Error "Staging deployment failed."
                    exit 1
                    '''
                }
            }
        }

        stage('Release') {
            steps {

                echo 'Promoting tested image to production...'

                bat '''
                docker tag %IMAGE_NAME%:%IMAGE_TAG% %IMAGE_NAME%:release-%BUILD_NUMBER%
                '''

                bat '''
                set RELEASE_TAG=release-%BUILD_NUMBER%

                docker compose -p taskmanager-production ^
                -f docker-compose.prod.yml down

                docker compose -p taskmanager-production ^
                -f docker-compose.prod.yml up -d

                docker compose -p taskmanager-production ^
                -f docker-compose.prod.yml ps
                '''

                powershell '''
                $maxAttempts = 5
                $attempt = 1

                while ($attempt -le $maxAttempts) {
                    try {
                        $response = Invoke-WebRequest `
                            -Uri "http://localhost:5001/health" `
                            -UseBasicParsing `
                            -TimeoutSec 5

                        if ($response.StatusCode -eq 200) {
                            Write-Host "Production release is healthy."
                            exit 0
                        }
                    }
                    catch {
                        Write-Host "Waiting for production application... attempt $attempt"
                    }

                    Start-Sleep -Seconds 5
                    $attempt++
                }

                Write-Error "Production release failed."
                exit 1
                '''
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
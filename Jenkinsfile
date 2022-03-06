pipeline {
    agent any
    triggers {
        githubPush()
    }
    environment { 
   NAME = "cassavagateway"
   projectName = "flextoeco"
   REGISTRYUSERNAME = "golide" 
   IMAGE_REPO_NAME="golidee"
   IMAGE_TAG="${VERSION}"
   REPOSITORY_URI = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com/${IMAGE_REPO_NAME}"
}
    stages {
        stage('Restore'){
           steps{
               sh 'dotnet restore PlatformService.sln'
            }
         }
        stage('Clean'){
           steps{
               sh 'dotnet clean PlatformService.sln --configuration Release'
            }
         }
        stage('Build'){
           steps{
               sh 'dotnet build PlatformService.sln --configuration Release --no-restore'
            }
         }
           stage('UnitTest'){
           steps{
               sh 'dotnet test --logger "trx;LogFileName=TestResults.trx"  --logger "xunit;LogFileName=TestResults.xml" --results-directory ./BuildReports/UnitTests /p:CollectCoverage=true /p:CoverletOutput=BuildReports/Coverage/ /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit.*]*"'
               sh 'dotnet ~/.nuget/packages/reportgenerator/4.8.13/tools/net5.0/ReportGenerator.dll "-reports:/var/lib/jenkins/workspace/FlexToEcocash_main/IntegrationTests/BuildReports/Coverage/coverage.cobertura.xml" "-targetdir:/var/lib/jenkins/workspace/FlexToEcocash_main/IntegrationTests/BuildReports/Coverage" "-reporttypes:HTML;HTMLSummary"'
            }
         }  
         stage ("UnitTest Report") {
             steps{               
                     publishHTML target: [
      allowMissing: false,
      alwaysLinkToLastBuild: true,
      keepAll: true,
      reportDir: '/var/lib/jenkins/workspace/FlexToEcocash_main/IntegrationTests/BuildReports/Coverage',
      reportFiles: 'index.html',
      reportName: 'Code Coverage'
      ]
               archiveArtifacts artifacts: 'IntegrationTests/BuildReports/Coverage/*.*'
               }
         }
            stage ("Perfomance Test") {
            steps {
                     build job: 'EcoToFlexPerfomanceTests'     
            }
         }
            stage ("Docker Build") {
            steps {
               sh  'cd /var/lib/jenkins/workspace/FlexToEcocash_main/FlexToEcocash'
               echo "Running ${VERSION} on ${env.JENKINS_URL}"              
               sh "docker build -t ${NAME} /var/lib/jenkins/workspace/FlexToEcocash_main/FlexToEcocash"             
               sh "docker tag ${REGISTRYUSERNAME}/${projectName}:latest ${REGISTRYUSERNAME}/${projectName}:${VERSION}"               
            }
         }
         stage ("K8S Deploy") {
            steps {     		                 			   
               sh "sudo k0s kubectl apply -f /var/lib/jenkins/workspace/FlexToEcocash_main/K8S/flextoeco-depl.yaml"             
               sh "sudo k0s kubectl set image deployments/flextoeco-depl flextoeco=golide/{projectName}:${VERSION}"               
            }
         }
		 stage ("ECR Login") {
            steps {
               sh "aws ecr get-login-password --region ${AWS_DEFAULT_REGION} | docker login --username AWS --password-stdin ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com"              
            }
         }
		 	 stage ("Push to ECR") {
            steps {
               sh "docker tag ${IMAGE_REPO_NAME}:${IMAGE_TAG} ${REPOSITORY_URI}:$IMAGE_TAG"
                sh "docker push ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com/${IMAGE_REPO_NAME}:${IMAGE_TAG}"              
            }
         }
 stage ("Performance Test") {
             steps{               
   sh 'docker exec 82fb8976535b bash'
   sh '/bin/sh -c cd /opt/apache-jmeter-5.4.1/bin'
   sh '/bin/sh -c echo pwd'
   sh '/bin/sh -c ./jmeter -n -t /home/getaccountperftest.jmx -l /home/Reports/LoadTestReport.csv -e -o /home/Reports/PerfHtmlReport'
   sh 'docker cp 82fb8976535b:/home/Reports/PerfHtmlReport /var/lib/jenkins/workspace/FlexToEcocash_main/IntegrationTests/BuildReports/Coverage bash'           
               }
         }
         stage ("Performance Test Report") {
             steps{               
                     publishHTML target: [
      allowMissing: false,
      alwaysLinkToLastBuild: true,
      keepAll: true,
      reportDir: '/var/lib/jenkins/workspace/FlexToEcocash_main/IntegrationTests/BuildReports/Coverage',
      reportFiles: 'PerfHtmlReport.html',
      reportName: 'PerfTest Report'
      ]   
      archiveArtifacts artifacts: 'IntegrationTests/BuildReports/Coverage/*.*' 
               }
         }

      
      }

}
 

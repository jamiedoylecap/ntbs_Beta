pipeline {
  agent { label 'windows' }
  stages {
    stage('run unit tests') {
      steps {
        powershell(script: '''
          Push-Location ntbs-service-tests
          Write-Output "Running unit tests"
          dotnet test
        ''')
      }
    }
    stage('build and publish') {
      steps {
        powershell(script: '''
          Push-Location ntbs-service
          Write-Output "Building frontend bundle"
          npm install
          npm run build:prod
          Write-Output "Building ntbs .net core application"
          dotnet publish -c Release
        ''')
      }
    }
    stage ('deploy to int') {
      steps {
        powershell(script: '''
          Write-Output "Releasing"
        ''')
        withCredentials([sshUserPrivateKey(credentialsId: 'dcee242f-4428-47c3-b106-f85f2e6acc2d', keyFileVariable: 'ssh_key_file')]) {
            powershell(script: '''
                Write-Output "Uploading release files to server stage"
                scp -rp -o StrictHostKeyChecking=no -i "$env:ssh_key_file" ./ntbs-service/bin/Release/netcoreapp2.2/ softwire@ntbs-int.uksouth.cloudapp.azure.com:C:/NTBS/stage
                Write-Output "Shutting down app"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell New-Item -Path 'C:/NTBS/netcoreapp2.2/publish/app_offline.htm' -ItemType File
                Write-Output "Remove old backups"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Remove-Item -Path 'C:/NTBS/backup' -Recurse
                Write-Output "Make a backup"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Copy-Item -Path 'C:/NTBS/netcoreapp2.2/' -Destination 'C:/NTBS/backup/' -Recurse
                Write-Output "Remove old files"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Remove-Item -Path 'C:/NTBS/netcoreapp2.2/publish' -Recurse -Exclude 'app_offline.htm'
                Write-Output "Copy over new files"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Copy-Item -Path 'C:/NTBS/stage/publish' -Destination 'C:/NTBS/netcoreapp2.2/' -Recurse
                Write-Output "Restarting app"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Remove-Item -Path 'C:/NTBS/netcoreapp2.2/publish/app_offline.htm'
                Write-Output "Removing stage files"
                ssh -i "$env:ssh_key_file" softwire@ntbs-int.uksouth.cloudapp.azure.com powershell Remove-Item -Path 'C:/NTBS/stage -Recurse'
                Write-Output "Done!"
            ''')
        }
      }
    }
  }
}

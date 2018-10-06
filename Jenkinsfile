node {
    stage('Clone repository') {
        checkout scm
    }

    stage('Initialize'){
        def dockerHome = tool 'docker'
        env.PATH = "${dockerHome}/bin:${env.PATH}"
    }

    stage('Build image') {
        sh 'docker build -t gcr.io/lhgames-2018/xx_suicideskwad69tbt2k17_xx:version-$BUILD_NUMBER .'
    }

    stage('Push image') {
        sh 'docker push gcr.io/lhgames-2018/xx_suicideskwad69tbt2k17_xx:version-$BUILD_NUMBER'
    }
}

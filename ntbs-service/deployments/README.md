# SSL Certificates
We use Let's Encrpyt for free certificates for the test environments
*Note: we tried using an automated cert-manager approach as per: https://docs.microsoft.com/en-us/azure/aks/ingress-tls, but couldn't get it to work. Hence the manual approach*

## Steps to create/renew certificate
1. The environment will have to go down for a while - verify this being OK with its users
1. Take note of the image tag currently deployed on the environment (`kubectl get deployments/ntbs-$env -o wide`) 
1. Replace the application with a bare-bones nginx container, e.g. by
    1. Replacing `spec.template.spec.containers[].image` with `"nginx:1.17.5"` in the deployment yaml file
    1. Remember the image uses port 80, so you might need to change the balancer's port to that, too:
        - in deployment: `spec.template.spec.containers[].ports[].containerPort` to 80
        - in service: `spec.ports[].targetPort` to 80
    1. Running `kubeclt apply -f <deployment yaml file>`
1. Wait for the nginx container to spin up - you should see a ngnix welcome page on the app environment's address
1. Go inside the container:
    1. Run `kubectl get pods` and note the pods name for your environment
    1. `kubectl exec -it $podName -- bin/bash`
1. In the container, follow the instructions from https://certbot.eff.org/lets-encrypt/debianbuster-nginx :
    1. `apt-get update`
    1. `apt-get install certbot python-certbot-nginx`
    1. `certbot certonly --nginx`, supplying the environment's domain and team email address as prompted
1. Copy the generated certs back out to your local machine. Note, the certbot output will tell you where they are, but it will refer to symlinks - you want to get the actual files, found in the archive folder:
    1. Go into a temporary folder on local machine
    1. `kubectl cp ${podName}:etc/letsencrypt/archive/$domain $domain`
1. Create/update the kubernetes secret from the certificate files, where secret name matches the name defined in the deployment's ingress yaml
(This part of the process is taken from https://docs.microsoft.com/en-us/azure/aks/ingress-own-tls):
    1. `kubectl delete secret $secretName`
    1. `kubectl create secret tls $secretName --key .\privkey1.pem --cert .\fullchain1.pem`
1. Return environment back to running the app, e.g. by running the release script with the image tag as noted in step 2.
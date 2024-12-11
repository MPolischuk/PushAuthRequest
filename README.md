# Push Authorization Request demo with .net 9

This is an example on how to implement OIDC Auth with Push Authorization Request using .net 9,


## How to run

This demo use KeyCloak as an authorization server.
In order to run the demo, create and run a local [keycloak server using docker](https://www.keycloak.org/getting-started/getting-started-docker)
Follow the instructions there and add a realm, clientId and a user to test.
Then configure the Program.cs file in the dotNetParOidc project with the corresponding auth url and clientId.
You will need to add the app URL to the client configuration in keycloak aswell (redirect url, etc)

Run the dotNetParOidc project.
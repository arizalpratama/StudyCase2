# StudyCase2
Microservices GraphQL

Connection String:
- Use 'Local' for development
- Use 'Database' for container
- Use 'localhost:9092' for kafka local
- Use 'my-release-kafka:9092' for kafka container

Kafka:
- add file docker-compose.yaml
- docker compose up

Docker Build and Push:
- docker build -t arizalpratama01/kafkaapp .
- docker push arizalpratama01/kafkaapp

- docker build -t arizalpratama01/kafkacreatetopics .
- docker push arizalpratama01/kafkacreatetopics

- docker build -t arizalpratama01/kafkalisteningapp .
- docker push arizalpratama01/kafkalisteningapp

- docker build -t arizalpratama01/twittorapi .
- docker push arizalpratama01/twittorapi

Kubernetes:
- kubectl get deployments
- kubectl get services
- kubectl get pods

Kubernetes Apply:
- kubectl apply -f kafkaapps-depl.yaml
- kubectl apply -f kafkacreatetopicss-depl.yaml
- kubectl apply -f kafkalisteningapps-depl.yaml
- kubectl apply -f twittorapis-depl.yaml

- kubectl apply local-pvc.yaml
- kubectl apply -f mssql-plat-depl.yaml
- kubectl apply -f ingress-srv.yaml

Kubernetes Delete:
- kubectl delete deployment kafkaapps-depl
- kubectl delete deployment kafkacreatetopicss-depl
- kubectl delete deployment kafkalisteningapps-depl
- kubectl delete deployment twittorapis-depl

- kubectl delete local-pvc
- kubectl delete deployment mssql-depl
- kubectl delete deployment ingress-srv

set -e
docker-compose down
docker-compose up --build -d

docker-compose run --rm tests 
set -e
docker-compose -f docker-compose.yml -f docker-compose.infra.yml down
docker-compose -f docker-compose.yml -f docker-compose.infra.yml up --build -d
docker-compose -f docker-compose.yml -f docker-compose.infra.yml run --rm tests
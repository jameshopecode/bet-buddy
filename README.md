# Bet Buddy



## How to run:
Run:
```sh
docker-compose up
```

## Init setup:

### Postgres DB Seeding

Go to DB in DB browser of your choice:
- `localhost:11005` with credentials:
  - user: `buddy`
  - password: `MNhl^2AT&E#EZ5G2`
- db name: `postgres`
- schema: `public`

Import sql from `BetBuddy.Backend/Sql/init.sql`.

### Vector DB Seeding

Make request to endpoint:
```http request
GET http://localhost/api/buddy/check
```

### LLM Model setup

- (required) After running docker compose go to ollama-server container and exec those commands for install necessary LLM models

```sh
ollama pull llama3.2:1b
ollama pull nomic-embed-text:v1.5
````

- (optional – faster, more accurate model) to pull cloud model – first signin 
  - if you don't have ollama account go to https://ollama.com and create free account, and generate api key
  - go to `BetBuddy.Backend/Ai/KernelBuilder.cs` and switch hardcoded model in `AddOllamaChatCompletion` to `gpt-oss:20b-cloud` 

```sh
ollama signin
ollama pull gpt-oss:20b-cloud
```

- It it also possible to download gpt-oss:20b (non-cloud version) if minimum of 16GB GPU VRAM as well as 16GB system storage is available
  - Remember to switch to this model in `BetBuddy.Backend/Ai/KernelBuilder.cs` -> `AddOllamaChatCompletion`

```sh
ollama pull gpt-oss:20b
```

## Access Web App

Web App is accessible via: `http://localhost`

For debugging or custom proxy (ex. external / dev server) configurable in `astro.config.mjs` run:
```sh
pnpm install // install dependencies
pnpm dev
```

Then access local setup either via `localhost` directly (nginx proxy) or `localhost:4321` (astro proxy pointing to external server).
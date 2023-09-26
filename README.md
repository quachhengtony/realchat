# Realchat

![Realchat Overall Architecture](realchat_overall_architecture.png?raw=true "Realchat Overall Architecture")

Realchat is an adaptable software platform designed for creating and serving multiple hybrid chatbots that blend the capabilities of predefined scripts with advanced language comprehension powered by the LLaMa-7B language model.

## Demo

[![Watch the video](https://img.youtube.com/vi/CcZzTyk-xlw/hqdefault.jpg)](https://www.youtube.com/embed/CcZzTyk-xlw)

## Description

Realchat offers a comprehensive solution for building and deploying hybrid chatbots that combine the strengths of predefined scripts and advanced language understanding. This unique synergy empowers chatbots to deliver intelligent responses while adhering to predefined conversation structures.

## Features

- Create and serve multiple hybrid chatbots.
- Manage organizations, chatbots, scripts, and knowledge bases.
- Upload .docx file to create knowledge bases.

## Running

1. Clone the repository.
   ```
   git clone https://github.com/quachhengtony/realchat.git
   ```
2. Run the docker-compose.dev.yml file.
   ```
   docker compose -f docker-compose.dev.yml up -d
   ```
3. Create the chatbots using the API. Follow the example in this [video](https://youtu.be/Bb9pECgnhso).
4. Use the chatbots. Follow the example in this [project](https://github.com/quachhengtony/realchat/tree/main/Presentation/WebApp).

## Technologies

- Frameworks: [.NET](https://dotnet.microsoft.com/en-us/).
- Databases: [MariaDB](https://mariadb.org/), [Milvus](https://milvus.io/).
- Encoder: [SBERT](https://www.sbert.net/).
- Object storage: [MinIO](https://min.io/).
- LLM: [LLaMA-7B](https://ai.meta.com/blog/large-language-model-llama-meta-ai/).
- Others: [Docker](https://www.docker.com/).

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)

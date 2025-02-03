FROM ubuntu:22.04

WORKDIR /app

COPY ./bin/Release/net8.0/publish/ /app/
COPY ./doc/schema.sql /app/schema.sql

RUN mkdir -p /data/avatar && \
    apt update && \
    apt install -y sqlite3 libicu70 libssl3 && \
    sqlite3 /data/wealthome.db < /app/schema.sql && \
    rm /app/schema.sql

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://0.0.0.0:5000" \
    ConnectionStrings__DefaultConnection="Data Source=/data/wealthome.db" \
    App__AvatarSaveDir="/data/avatar"

CMD ["./Gacfox.Wealthome"]

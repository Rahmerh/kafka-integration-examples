FROM node:lts-alpine

RUN apk add openjdk11
RUN npm i -g firebase-tools

WORKDIR /app

CMD [ "firebase", "emulators:start", "--only", "firestore", "--import", "/data", "--export-on-exit", "/data/export-on-close" ]

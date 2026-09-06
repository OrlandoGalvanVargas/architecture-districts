FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci
COPY . .

ARG VITE_API_URL=/api
ARG VITE_API_TIMEOUT=30000
ARG VITE_ENABLE_LOGGING=false
ARG VITE_ENABLE_MOCK=false
ARG VITE_APP_ENV=production

ENV VITE_API_URL=$VITE_API_URL
ENV VITE_API_TIMEOUT=$VITE_API_TIMEOUT
ENV VITE_ENABLE_LOGGING=$VITE_ENABLE_LOGGING
ENV VITE_ENABLE_MOCK=$VITE_ENABLE_MOCK
ENV VITE_APP_ENV=$VITE_APP_ENV

RUN npm run validate
RUN npm run build


FROM nginx:1.25-alpine AS final

COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html

ENV BACKEND_PRIVATE_URL=http://facilityos-api-prod:8080

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
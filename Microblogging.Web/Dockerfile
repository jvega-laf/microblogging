FROM node:20-alpine as builder

WORKDIR /app
COPY . .
RUN npm install && npm run build

FROM docker.io/library/nginx:latest
COPY --from=builder /app/dist /usr/share/nginx/html
# COPY nginx.conf /etc/nginx/conf.d/default.conf
# COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

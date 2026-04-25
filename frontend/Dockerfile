# -------- Stage 1: Install deps --------
FROM node:20 AS deps

WORKDIR /app
COPY package*.json ./
RUN npm install


# -------- Stage 2: Dev server --------
FROM node:20

WORKDIR /app

# Copy node_modules from previous stage
COPY --from=deps /app/node_modules ./node_modules

# Copy source code
COPY . .

EXPOSE 4200

CMD ["npx", "ng", "serve", "--host", "0.0.0.0", "--port", "4200"]
CREATE TABLE IF NOT EXISTS "t_transfer_type" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_t_transfer_type" PRIMARY KEY AUTOINCREMENT,
    "name" TEXT NOT NULL,
    "inout_type" INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS "t_user" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_t_user" PRIMARY KEY AUTOINCREMENT,
    "user_name" TEXT NOT NULL,
    "display_name" TEXT NULL,
    "email" TEXT NULL,
    "avatar_url" TEXT NULL,
    "role_code" TEXT NOT NULL,
    "password_hash" TEXT NOT NULL,
    "status" INTEGER NOT NULL,
    "created_at" TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS "t_account" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_t_account" PRIMARY KEY AUTOINCREMENT,
    "account_name" TEXT NOT NULL,
    "account_type" INTEGER NOT NULL,
    "balance" TEXT NOT NULL DEFAULT '0.0',
    "user_id" INTEGER NOT NULL,
    "created_at" TEXT NOT NULL,
    CONSTRAINT "FK_t_account_t_user_user_id" FOREIGN KEY ("user_id") REFERENCES "t_user" ("id") ON DELETE CASCADE
);
CREATE INDEX "IX_t_account_user_id" ON "t_account" ("user_id");
CREATE UNIQUE INDEX "IX_t_user_user_name" ON "t_user" ("user_name");
CREATE TABLE IF NOT EXISTS "t_transfer" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_t_transfer" PRIMARY KEY AUTOINCREMENT,
    "created_at" TEXT NOT NULL,
    "description" TEXT NULL,
    "title" TEXT NOT NULL,
    "transfer_type_id" INTEGER NOT NULL,
    "user_id" INTEGER NOT NULL,
    CONSTRAINT "FK_t_transfer_t_transfer_type_transfer_type_id" FOREIGN KEY ("transfer_type_id") REFERENCES "t_transfer_type" ("id") ON DELETE CASCADE,
    CONSTRAINT "FK_t_transfer_t_user_user_id" FOREIGN KEY ("user_id") REFERENCES "t_user" ("id") ON DELETE CASCADE
);
CREATE INDEX "IX_t_transfer_transfer_type_id" ON "t_transfer" ("transfer_type_id");
CREATE INDEX "IX_t_transfer_user_id" ON "t_transfer" ("user_id");
CREATE TABLE IF NOT EXISTS "t_flow" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_t_flow" PRIMARY KEY AUTOINCREMENT,
    "account_id" INTEGER NOT NULL,
    "amount" TEXT NOT NULL,
    "created_at" TEXT NOT NULL,
    "transfer_id" INTEGER NOT NULL,
    CONSTRAINT "FK_t_flow_t_account_account_id" FOREIGN KEY ("account_id") REFERENCES "t_account" ("id") ON DELETE CASCADE,
    CONSTRAINT "FK_t_flow_t_transfer_transfer_id" FOREIGN KEY ("transfer_id") REFERENCES "t_transfer" ("id") ON DELETE CASCADE
);
CREATE INDEX "IX_t_flow_account_id" ON "t_flow" ("account_id");
CREATE INDEX "IX_t_flow_transfer_id" ON "t_flow" ("transfer_id");
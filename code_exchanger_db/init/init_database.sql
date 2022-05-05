CREATE TABLE "Content"(
	"code" character varying(262144) not null,
	"creation_time" date not null,
	"authorID" bigint,
	"language" smallint not null,
	"password" bytea,
	"ID" bigint not null,
	"link" character varying(37) not null,
	primary key ("ID", "link")
);

CREATE TABLE "Users"(
	"ID" bigint not null,
	"username" character varying(16) not null,
	"password" bytea not null,
	primary key ("ID", "username")
);
CREATE TABLE "Content"(
	"code" character varying(262144) not null,
	"creation_time" date not null,
	"authorID" bigint,
	"language" smallint not null,
	"password" character varying(262144),
	"ID" bigint not null,
	"link" character varying(37) not null,
	primary key ("ID", "link")
);

CREATE TABLE "Users"(
	"ID" bigint not null,
	"username" character varying(16) not null,
	"password" character varying(262144) not null,
	primary key ("ID", "username")
);
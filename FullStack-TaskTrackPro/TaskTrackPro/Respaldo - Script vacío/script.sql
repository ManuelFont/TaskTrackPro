-- DROP SCHEMA dbo;

-- TaskTrack2.dbo.Usuarios definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.Usuarios;

CREATE TABLE TaskTrack2.dbo.Usuarios (
                                         Email nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         Apellido nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         Password nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         FechaNacimiento datetime2 NOT NULL,
                                         RolesSerializados nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                         CONSTRAINT PK_Usuarios PRIMARY KEY (Email)
);


-- TaskTrack2.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.[__EFMigrationsHistory];

CREATE TABLE TaskTrack2.dbo.[__EFMigrationsHistory] (
                                                        MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );


-- TaskTrack2.dbo.Notificaciones definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.Notificaciones;

CREATE TABLE TaskTrack2.dbo.Notificaciones (
                                               Id int IDENTITY(1,1) NOT NULL,
                                               Mensaje nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                               Fecha datetime2 NOT NULL,
                                               Vista bit NOT NULL,
                                               UsuarioEmail nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                               CONSTRAINT PK_Notificaciones PRIMARY KEY (Id),
                                               CONSTRAINT FK_Notificaciones_Usuarios_UsuarioEmail FOREIGN KEY (UsuarioEmail) REFERENCES TaskTrack2.dbo.Usuarios(Email) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_Notificaciones_UsuarioEmail ON TaskTrack2.dbo.Notificaciones (  UsuarioEmail ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.Proyecto definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.Proyecto;

CREATE TABLE TaskTrack2.dbo.Proyecto (
                                         Nombre nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         Descripcion nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                         FechaInicioEstimada datetime2 NOT NULL,
                                         LiderEmail nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                         CONSTRAINT PK_Proyecto PRIMARY KEY (Nombre),
                                         CONSTRAINT FK_Proyecto_Usuarios_LiderEmail FOREIGN KEY (LiderEmail) REFERENCES TaskTrack2.dbo.Usuarios(Email)
);
CREATE NONCLUSTERED INDEX IX_Proyecto_LiderEmail ON TaskTrack2.dbo.Proyecto (  LiderEmail ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.ProyectoUsuario definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.ProyectoUsuario;

CREATE TABLE TaskTrack2.dbo.ProyectoUsuario (
                                                ListaProyectosNombre nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                ListaUsuariosEmail nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                CONSTRAINT PK_ProyectoUsuario PRIMARY KEY (ListaProyectosNombre,ListaUsuariosEmail),
                                                CONSTRAINT FK_ProyectoUsuario_Proyecto_ListaProyectosNombre FOREIGN KEY (ListaProyectosNombre) REFERENCES TaskTrack2.dbo.Proyecto(Nombre) ON DELETE CASCADE,
                                                CONSTRAINT FK_ProyectoUsuario_Usuarios_ListaUsuariosEmail FOREIGN KEY (ListaUsuariosEmail) REFERENCES TaskTrack2.dbo.Usuarios(Email) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_ProyectoUsuario_ListaUsuariosEmail ON TaskTrack2.dbo.ProyectoUsuario (  ListaUsuariosEmail ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.Recurso definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.Recurso;

CREATE TABLE TaskTrack2.dbo.Recurso (
                                        Nombre nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                        Tipo nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                        Descripcion nvarchar(250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                        Id int IDENTITY(1,1) NOT NULL,
                                        ProyectoNombre nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                                        Funcionalidad nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                        UtilizadoHasta datetime2 NULL,
                                        FechaAsignacion datetime2 NULL,
                                        Capacidad int DEFAULT 0 NOT NULL,
                                        Usos int DEFAULT 0 NOT NULL,
                                        CONSTRAINT PK_Recurso PRIMARY KEY (Id),
                                        CONSTRAINT FK_Recurso_Proyecto FOREIGN KEY (ProyectoNombre) REFERENCES TaskTrack2.dbo.Proyecto(Nombre)
);


-- TaskTrack2.dbo.Tarea definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.Tarea;

CREATE TABLE TaskTrack2.dbo.Tarea (
                                      Titulo nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                      Realizada bit NOT NULL,
                                      ProyectoNombre nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                      FechaEjecucion datetime2 NULL,
                                      DuracionEnDias int NOT NULL,
                                      Descripcion nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                      FechaInicioForzada datetime2 NULL,
                                      CONSTRAINT PK_Tarea PRIMARY KEY (Titulo),
                                      CONSTRAINT FK_Tarea_Proyecto_ProyectoNombre FOREIGN KEY (ProyectoNombre) REFERENCES TaskTrack2.dbo.Proyecto(Nombre) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_Tarea_ProyectoNombre ON TaskTrack2.dbo.Tarea (  ProyectoNombre ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.TareaDependencia definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.TareaDependencia;

CREATE TABLE TaskTrack2.dbo.TareaDependencia (
                                                 DependenciaId nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                 TareaId nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                                 CONSTRAINT PK_TareaDependencia PRIMARY KEY (DependenciaId,TareaId),
                                                 CONSTRAINT FK_TareaDependencia_Tarea_DependenciaId FOREIGN KEY (DependenciaId) REFERENCES TaskTrack2.dbo.Tarea(Titulo) ON DELETE CASCADE,
                                                 CONSTRAINT FK_TareaDependencia_Tarea_TareaId FOREIGN KEY (TareaId) REFERENCES TaskTrack2.dbo.Tarea(Titulo)
);
CREATE NONCLUSTERED INDEX IX_TareaDependencia_TareaId ON TaskTrack2.dbo.TareaDependencia (  TareaId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.TareaRecurso definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.TareaRecurso;

CREATE TABLE TaskTrack2.dbo.TareaRecurso (
                                             RecursosId int NOT NULL,
                                             TareasTitulo nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                                             CONSTRAINT PK_TareaRecurso PRIMARY KEY (RecursosId,TareasTitulo),
                                             CONSTRAINT FK_TareaRecurso_Recurso_RecursosId FOREIGN KEY (RecursosId) REFERENCES TaskTrack2.dbo.Recurso(Id) ON DELETE CASCADE,
                                             CONSTRAINT FK_TareaRecurso_Tarea_TareasTitulo FOREIGN KEY (TareasTitulo) REFERENCES TaskTrack2.dbo.Tarea(Titulo) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_TareaRecurso_TareasTitulo ON TaskTrack2.dbo.TareaRecurso (  TareasTitulo ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- TaskTrack2.dbo.RangosFecha definition

-- Drop table

-- DROP TABLE TaskTrack2.dbo.RangosFecha;

CREATE TABLE TaskTrack2.dbo.RangosFecha (
                                            Id int IDENTITY(1,1) NOT NULL,
                                            Desde datetime2 NOT NULL,
                                            Hasta datetime2 NOT NULL,
                                            RecursoId int NOT NULL,
                                            TareaTitulo nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
                                            CONSTRAINT PK_RangosFecha PRIMARY KEY (Id),
                                            CONSTRAINT FK_RangosFecha_Recurso_RecursoId FOREIGN KEY (RecursoId) REFERENCES TaskTrack2.dbo.Recurso(Id) ON DELETE CASCADE,
                                            CONSTRAINT FK_RangosFecha_Tarea_TareaTitulo FOREIGN KEY (TareaTitulo) REFERENCES TaskTrack2.dbo.Tarea(Titulo) ON DELETE CASCADE
);
CREATE NONCLUSTERED INDEX IX_RangosFecha_RecursoId ON TaskTrack2.dbo.RangosFecha (  RecursoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_RangosFecha_TareaTitulo ON TaskTrack2.dbo.RangosFecha (  TareaTitulo ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
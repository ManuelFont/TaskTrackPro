INSERT INTO TaskTrack2.dbo.Notificaciones (Mensaje,Fecha,Vista,UsuarioEmail) VALUES
	 (N'Fuiste asignado como Líder del proyecto "Gestion interna 2025".','2025-06-17 16:07:29.2817238',0,N'liderproyecto2@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Optimizacion de recursos".','2025-06-17 16:08:01.2188328',0,N'liderproyecto1@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Migracion a la nube".','2025-06-17 16:08:22.4589199',0,N'liderproyecto3@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "App Cliente Web".','2025-06-17 16:08:49.9189162',0,N'liderproyecto3@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Plataforma educativa".','2025-06-17 16:09:13.9886290',0,N'liderproyecto2@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "TESTEAR ALGORITMO RECURSOS".','2025-06-17 16:21:01.7983154',0,N'liderproyecto1@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Modulo de seguridad".','2025-06-17 16:21:40.8664390',0,N'liderproyecto1@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Dashboard operativo".','2025-06-17 16:22:05.0914821',0,N'liderproyecto3@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Mantenimiento servidores".','2025-06-17 16:22:29.5900655',0,N'liderproyecto2@mail.com'),
	 (N'Fuiste asignado como Líder del proyecto "Proyecto XR5".','2025-06-17 16:23:06.4710721',0,N'liderproyecto1@mail.com');
INSERT INTO TaskTrack2.dbo.Proyecto (Nombre,Descripcion,FechaInicioEstimada,LiderEmail) VALUES
	 (N'App Cliente Web',N'.','2026-01-18 00:00:00.0000000',N'liderproyecto3@mail.com'),
	 (N'Dashboard operativo',N'.','2026-02-17 00:00:00.0000000',N'liderproyecto3@mail.com'),
	 (N'Gestion interna 2025',N'.','2025-08-08 00:00:00.0000000',N'liderproyecto2@mail.com'),
	 (N'Mantenimiento servidores',N'.','2025-10-25 00:00:00.0000000',N'liderproyecto2@mail.com'),
	 (N'Migracion a la nube',N'.','2025-11-21 00:00:00.0000000',N'liderproyecto3@mail.com'),
	 (N'Modulo de seguridad',N'.','2025-12-31 00:00:00.0000000',N'liderproyecto1@mail.com'),
	 (N'Optimizacion de recursos',N'.','2025-10-17 00:00:00.0000000',N'liderproyecto1@mail.com'),
	 (N'Plataforma educativa',N'.','2025-09-05 00:00:00.0000000',N'liderproyecto2@mail.com'),
	 (N'Proyecto XR5',N'.','2025-09-11 00:00:00.0000000',N'liderproyecto1@mail.com'),
	 (N'TESTEAR ALGORITMO RECURSOS',N'En este proyecto se testea el algoritmo de recursos.','2025-09-20 00:00:00.0000000',N'liderproyecto1@mail.com');
INSERT INTO TaskTrack2.dbo.ProyectoUsuario (ListaProyectosNombre,ListaUsuariosEmail) VALUES
	 (N'App Cliente Web',N'adminProyecto@mail.com'),
	 (N'Dashboard operativo',N'adminProyecto@mail.com'),
	 (N'Gestion interna 2025',N'adminProyecto@mail.com'),
	 (N'Mantenimiento servidores',N'adminProyecto@mail.com'),
	 (N'Migracion a la nube',N'adminProyecto@mail.com'),
	 (N'Modulo de seguridad',N'adminProyecto@mail.com'),
	 (N'Optimizacion de recursos',N'adminProyecto@mail.com'),
	 (N'Plataforma educativa',N'adminProyecto@mail.com'),
	 (N'Proyecto XR5',N'adminProyecto@mail.com'),
	 (N'TESTEAR ALGORITMO RECURSOS',N'adminProyecto@mail.com');
INSERT INTO TaskTrack2.dbo.ProyectoUsuario (ListaProyectosNombre,ListaUsuariosEmail) VALUES
	 (N'Modulo de seguridad',N'liderproyecto1@mail.com'),
	 (N'Optimizacion de recursos',N'liderproyecto1@mail.com'),
	 (N'Proyecto XR5',N'liderproyecto1@mail.com'),
	 (N'TESTEAR ALGORITMO RECURSOS',N'liderproyecto1@mail.com'),
	 (N'Gestion interna 2025',N'liderproyecto2@mail.com'),
	 (N'Mantenimiento servidores',N'liderproyecto2@mail.com'),
	 (N'Plataforma educativa',N'liderproyecto2@mail.com'),
	 (N'App Cliente Web',N'liderproyecto3@mail.com'),
	 (N'Dashboard operativo',N'liderproyecto3@mail.com'),
	 (N'Migracion a la nube',N'liderproyecto3@mail.com');
INSERT INTO TaskTrack2.dbo.ProyectoUsuario (ListaProyectosNombre,ListaUsuariosEmail) VALUES
	 (N'App Cliente Web',N'miembro1@mail.com'),
	 (N'Modulo de seguridad',N'miembro1@mail.com'),
	 (N'Modulo de seguridad',N'miembro10@mail.com'),
	 (N'Plataforma educativa',N'miembro10@mail.com'),
	 (N'App Cliente Web',N'miembro2@mail.com'),
	 (N'Modulo de seguridad',N'miembro2@mail.com'),
	 (N'Gestion interna 2025',N'miembro3@mail.com'),
	 (N'Mantenimiento servidores',N'miembro3@mail.com'),
	 (N'Proyecto XR5',N'miembro3@mail.com'),
	 (N'TESTEAR ALGORITMO RECURSOS',N'miembro4@mail.com');
INSERT INTO TaskTrack2.dbo.ProyectoUsuario (ListaProyectosNombre,ListaUsuariosEmail) VALUES
	 (N'App Cliente Web',N'miembro5@mail.com'),
	 (N'Dashboard operativo',N'miembro5@mail.com'),
	 (N'Modulo de seguridad',N'miembro5@mail.com'),
	 (N'Optimizacion de recursos',N'miembro5@mail.com'),
	 (N'Proyecto XR5',N'miembro5@mail.com'),
	 (N'Modulo de seguridad',N'miembro7@mail.com'),
	 (N'Plataforma educativa',N'miembro8@mail.com'),
	 (N'Gestion interna 2025',N'miembro9@mail.com'),
	 (N'TESTEAR ALGORITMO RECURSOS',N'miembro9@mail.com');
INSERT INTO TaskTrack2.dbo.Recurso (Nombre,Tipo,Descripcion,ProyectoNombre,Funcionalidad,UtilizadoHasta,FechaAsignacion,Capacidad,Usos) VALUES
	 (N'Maria',N'Programador',N'Programadora FrontEnd',NULL,N'C++',NULL,NULL,1,0),
	 (N'Pablo',N'Programador',N'Programador FrontEnd',NULL,N'C++',NULL,NULL,1,0),
	 (N'Ansus 7001',N'Computadora',N'Año 2023',NULL,N'Hardware',NULL,NULL,2,0),
	 (N'Bravo 938',N'Computadora',N'Año 2025',NULL,N'Hardware',NULL,NULL,5,0),
	 (N'Servidor 90',N'Servidores',N'Endpoint 9.2.11',NULL,N'Red',NULL,NULL,2,0),
	 (N'Fernando',N'Reparador PC',N'Horario: 12:00 - 18:00',NULL,N'Windows',NULL,NULL,1,0);
INSERT INTO TaskTrack2.dbo.Tarea (Titulo,Realizada,ProyectoNombre,FechaEjecucion,DuracionEnDias,Descripcion,FechaInicioForzada) VALUES
	 (N'Administrar recursos',0,N'Plataforma educativa',NULL,3,N'.',NULL),
	 (N'Almacenamiento',0,N'Modulo de seguridad',NULL,1,N'.',NULL),
	 (N'Ciberseguirdad',0,N'Modulo de seguridad',NULL,2,N'.',NULL),
	 (N'Crear base de datos',0,N'Gestion interna 2025',NULL,2,N'.',NULL),
	 (N'Crear entorno',0,N'Migracion a la nube',NULL,3,N'.',NULL),
	 (N'Crear web',0,N'Plataforma educativa',NULL,4,N'.',NULL),
	 (N'Documentacion',0,N'Dashboard operativo',NULL,1,N'.',NULL),
	 (N'Entendimiento',0,N'App Cliente Web',NULL,2,N'.',NULL),
	 (N'Migrar datos',0,N'Gestion interna 2025',NULL,3,N'.',NULL),
	 (N'Preparado de codigo',0,N'App Cliente Web',NULL,1,N'.',NULL);
INSERT INTO TaskTrack2.dbo.Tarea (Titulo,Realizada,ProyectoNombre,FechaEjecucion,DuracionEnDias,Descripcion,FechaInicioForzada) VALUES
	 (N'Preparar ppt',0,N'Plataforma educativa',NULL,2,N'.',NULL),
	 (N'Procesado',0,N'Dashboard operativo',NULL,1,N'.',NULL),
	 (N'Proceso optimización',0,N'Optimizacion de recursos',NULL,3,N'.',NULL),
	 (N'Registrar recrusos',0,N'Optimizacion de recursos',NULL,4,N'.',NULL),
	 (N'Requerimientos',0,N'Dashboard operativo',NULL,3,N'.',NULL),
	 (N'Resetar CPU',0,N'Migracion a la nube',NULL,1,N'.',NULL),
	 (N'Reunion ',0,N'App Cliente Web',NULL,1,N'.',NULL),
	 (N'Reunirse ',0,N'Plataforma educativa',NULL,1,N'.',NULL),
	 (N'Sistema',0,N'Dashboard operativo',NULL,2,N'.',NULL),
	 (N'Subir archivos',0,N'Migracion a la nube',NULL,1,N'.',NULL);
INSERT INTO TaskTrack2.dbo.Tarea (Titulo,Realizada,ProyectoNombre,FechaEjecucion,DuracionEnDias,Descripcion,FechaInicioForzada) VALUES
	 (N'Testeo ',0,N'Modulo de seguridad',NULL,2,N'.',NULL);
INSERT INTO TaskTrack2.dbo.TareaDependencia (DependenciaId,TareaId) VALUES
	 (N'Testeo ',N'Ciberseguirdad'),
	 (N'Migrar datos',N'Crear base de datos'),
	 (N'Subir archivos',N'Crear entorno'),
	 (N'Administrar recursos',N'Crear web'),
	 (N'Reunion ',N'Entendimiento'),
	 (N'Proceso optimización',N'Registrar recrusos'),
	 (N'Documentacion',N'Requerimientos'),
	 (N'Procesado',N'Requerimientos'),
	 (N'Resetar CPU',N'Subir archivos'),
	 (N'Almacenamiento',N'Testeo ');
INSERT INTO TaskTrack2.dbo.Usuarios (Email,Nombre,Apellido,Password,FechaNacimiento,RolesSerializados) VALUES
	 (N'adminProyecto@mail.com',N'Administrador',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'AdminProyecto'),
	 (N'adminSistema@mail.com',N'Administrador',N'Sistema',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','1995-05-05 00:00:00.0000000',N'AdminSistema'),
	 (N'liderproyecto1@mail.com',N'Lider',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'LiderProyecto'),
	 (N'liderproyecto2@mail.com',N'Lider',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'LiderProyecto'),
	 (N'liderproyecto3@mail.com',N'Lider',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'LiderProyecto'),
	 (N'miembro1@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro10@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro2@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro3@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro4@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto');
INSERT INTO TaskTrack2.dbo.Usuarios (Email,Nombre,Apellido,Password,FechaNacimiento,RolesSerializados) VALUES
	 (N'miembro5@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro6@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro7@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro8@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto'),
	 (N'miembro9@mail.com',N'Miembro',N'Proyecto',N'da05b1c555994f4d34f0b5e14fd340c7fdb0a263a8cf41625cc2200ea43c3b92','2000-01-01 00:00:00.0000000',N'MiembroProyecto');

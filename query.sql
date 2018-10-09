use myCareer
CREATE TABLE [dbo].[jobseeker_info](
[js_id] [int] IDENTITY(1,1),
[full_name] [varchar] (50),
[current_address] [varchar] (100),
[email] [varchar] (100),
[email_verified] [varchar] (10),
[contact] [varchar] (15),
[contact_verified] [varchar] (10),
[created_at] [datetime],
[updated_at] [datetime],
[status] varchar (10),
[facebooklogin_status] varchar (10),
[facebook_id] varchar(50),
[user_name] varchar(50),
[password] varchar(50)
CONSTRAINT pk_js_info PRIMARY KEY (js_id)
)
CREATE TABLE [dbo].[users]
(
	[user_id] int IDENTITY(1,1),
	[user_name] varchar (50),
	[password] varchar (50),
	[role] varchar (50),
	CONSTRAINT pk_user PRIMARY KEY (user_id)

)
CREATE TABLE [dbo].[company_type]
(
	[id] int IDENTITY(1,1),
	[category_name] varchar(100),
	[status] varchar(50)
	constraint pk_ctype_id PRIMARY KEY (id)

)
CREATE TABLE [dbo].[registration_type]
(
	[id] int IDENTITY(1,1),
	[registration_name] varchar(100),
	[status] varchar(50)
	constraint pk_rtype_id PRIMARY KEY (id)

)
create table [dbo].[employer_info]
(
	[emp_id] int IDENTITY(1,1),
	[company_name]varchar(100),
	[company_type] int,
	[phone] varchar(20),
	[address] varchar (100),
	[description] text, 
	[reg_type] int,
	[name] varchar (50),
	[email] varchar(50),
	[contact] varchar (50),
	[created_at] datetime,
	[updated_at] datetime,
	[status] varchar(50),
	[logo] varchar(150),
	[login_info] int,
	constraint pk_employee PRIMARY KEY (emp_id)
)
create table [dbo].[job]
(
	job_id int identity(1,1),
	title text,
	submission_date date,
	category varchar(200),
	description text,
	job_type varchar(200),
	job_level varchar(50),
	gender varchar(50),
	salaryfrom float,
	salaryto float,
	experience int,
	state varchar(50),
	district varchar (50),
	city varchar (50),
	postalcode varchar (50),
	full_address text,
	constraint pk_job PRIMARY KEY (job_id)
)
	


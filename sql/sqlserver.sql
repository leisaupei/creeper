USE [master]
GO
/****** Object:  Database [demo]    Script Date: 2021/9/2 17:26:11 ******/
CREATE DATABASE [demo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'demo', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\demo.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'demo_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\demo_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [demo] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [demo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [demo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [demo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [demo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [demo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [demo] SET ARITHABORT OFF 
GO
ALTER DATABASE [demo] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [demo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [demo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [demo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [demo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [demo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [demo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [demo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [demo] SET  DISABLE_BROKER 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [demo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [demo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [demo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [demo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [demo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [demo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [demo] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [demo] SET  MULTI_USER 
GO
ALTER DATABASE [demo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [demo] SET DB_CHAINING OFF 
GO
ALTER DATABASE [demo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [demo] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [demo] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [demo] SET QUERY_STORE = OFF
GO
USE [demo]
GO
/****** Object:  Table [dbo].[GuidComposite]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuidComposite](
	[Uid] [uniqueidentifier] NOT NULL,
	[Gid] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Uid] ASC,
	[Gid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuidPk]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuidPk](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IdenGuidComposite]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IdenGuidComposite](
	[Iid] [int] IDENTITY(1,1) NOT NULL,
	[Uid] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Iid] ASC,
	[Uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IdenPk]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IdenPk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IdenPkUniCol]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IdenPkUniCol](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UniqueColumn] [varchar](255) NOT NULL,
	[Name] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [idxUniqueCol] UNIQUE NONCLUSTERED 
(
	[UniqueColumn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[CreateTime] [bigint] NOT NULL,
	[StuNo] [int] NOT NULL,
 CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TypeTest]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeTest](
	[BigintType] [bigint] NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NvarcharType] [nvarchar](10) NULL,
	[BitType] [bit] NULL,
	[BinaryType] [binary](50) NULL,
	[CharType] [char](10) NULL,
	[DateType] [date] NULL,
	[DateTimeType] [datetime] NULL,
	[DateTime2Type] [datetime2](7) NULL,
	[DateTimeOffsetType] [datetimeoffset](7) NULL,
	[DecimalType] [decimal](10, 4) NULL,
	[FloatType] [float] NULL,
	[GeographyType] [geography] NULL,
	[GeometryType] [geometry] NULL,
	[HierarchyidType] [hierarchyid] NULL,
	[ImageType] [image] NULL,
	[IntType] [int] NULL,
	[MoneyType] [money] NULL,
	[NcharType] [nchar](10) NULL,
	[NtextType] [ntext] NULL,
	[NumericType] [numeric](10, 2) NULL,
	[RealType] [real] NULL,
	[SmallDateTimeType] [smalldatetime] NULL,
	[SmallIntType] [smallint] NULL,
	[SmallMoneyType] [smallmoney] NULL,
	[SqlVariantType] [sql_variant] NULL,
	[TextType] [text] NULL,
	[TimeType] [time](7) NULL,
	[TimestampType] [timestamp] NULL,
	[TinyIntType] [tinyint] NULL,
	[UniqueIdentifierType] [uniqueidentifier] NULL,
	[VarbinaryType] [varbinary](50) NULL,
	[VarcharType] [varchar](50) NULL,
	[XmlType] [xml] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UniTest]    Script Date: 2021/9/2 17:26:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UniTest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UniqueColumn] [varchar](255) NOT NULL,
	[IdxUniqueColumn] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [Uni_like] UNIQUE NONCLUSTERED 
(
	[UniqueColumn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_uni_col]    Script Date: 2021/9/2 17:26:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_uni_col] ON [dbo].[UniTest]
(
	[IdxUniqueColumn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间戳' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'学号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'StuNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明测试' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TypeTest', @level2type=N'COLUMN',@level2name=N'DecimalType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CLR测试表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TypeTest'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'测试唯一键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UniTest'
GO
USE [master]
GO
ALTER DATABASE [demo] SET  READ_WRITE 
GO

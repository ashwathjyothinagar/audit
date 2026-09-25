
/****** Object:  Table [dbo].[AuditCheckCrns]    Script Date: 9/24/2026 9:10:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditCheckCrns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditCheckId] [int] NOT NULL,
	[CrnId] [int] NOT NULL,
 CONSTRAINT [PK_AuditCheckCrns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditChecks]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditChecks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TaskId] [int] NULL,
	[OfficeId] [int] NULL,
	[OrderNumberSeries] [nvarchar](1000) NULL,
	[AuditCheckValues] [nvarchar](max) NULL,
 CONSTRAINT [PK__AuditChecks__3214EC07290B028D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditCheckTasks]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditCheckTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditCheckId] [int] NOT NULL,
	[TaskId] [int] NOT NULL,
 CONSTRAINT [PK_AuditCheckTasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditCheckValues]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditCheckValues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditCheckId] [int] NOT NULL,
	[TitleOrderId] [int] NOT NULL,
	[AuditCheckValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_AuditCheckValues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditDocuments]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditDocuments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditId] [int] NOT NULL,
	[DocumentPath] [varchar](1000) NOT NULL,
	[FileName] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_AuditDocuments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditErrorCategories]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditErrorCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](300) NOT NULL,
 CONSTRAINT [PK_AuditErrorCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditErrorJson]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditErrorJson](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderErrorJson] [varchar](max) NOT NULL,
	[TaskId] [int] NOT NULL,
	[AuditId] [int] NOT NULL,
 CONSTRAINT [PK_AuditErrorJson] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditErrorTypes]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditErrorTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[AuditErrorCategoryId] [int] NOT NULL,
	[IsCritical] [bit] NOT NULL,
 CONSTRAINT [PK_AuditErrorTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditHoldReasons]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditHoldReasons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_AuditHoldReasons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditHolds]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditHolds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditId] [int] NOT NULL,
	[AuditTaskId] [int] NOT NULL,
	[HoldBy] [int] NOT NULL,
	[HoldDateTime] [datetime] NOT NULL,
	[HoldRelievedBy] [int] NULL,
	[HoldRelivedDateTime] [datetime] NULL,
	[Comment] [varchar](1000) NULL,
 CONSTRAINT [PK_AuditHolds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditRejectReasons]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditRejectReasons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_AuditRejectReasons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditRejects]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditRejects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditId] [int] NOT NULL,
	[RejectedBy] [int] NOT NULL,
	[RejectedDateTime] [datetime] NOT NULL,
	[AcceptedBy] [int] NULL,
	[AcceptedDateTime] [datetime] NULL,
	[Reason] [varchar](1000) NULL,
 CONSTRAINT [PK_AuditRejects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditRequestTypes]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditRequestTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](300) NOT NULL,
 CONSTRAINT [PK_AuditRequestTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Audits]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](200) NOT NULL,
	[OwnerNames] [varchar](300) NULL,
	[PropertyAddress] [varchar](1000) NULL,
	[APNNo] [varchar](200) NULL,
	[RequestTypeId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[TaskId] [int] NOT NULL,
	[SenderId] [int] NULL,
	[CrnId] [int] NOT NULL,
	[RequestedByEmailId] [varchar](500) NULL,
	[ClientInstructions] [varchar](max) NULL,
	[CheckOwnerSearchCount] [int] NULL,
	[CheckDirectHitUploaded] [bit] NULL,
	[UploadDateTime] [datetime] NULL,
	[DateCreated] [datetime] NULL,
	[UserCreated] [int] NULL,
	[DateModified] [datetime] NULL,
	[UserModified] [int] NULL,
	[IsQcRequired] [bit] NULL,
	[EffectiveDateChanged] [bit] NULL,
	[AnyChangeInVesting] [bit] NULL,
	[AnyUpdateOnTaxInformation] [bit] NULL,
	[AnyUpdateOnNewPIDocs] [bit] NULL,
	[AnyUpdateOnNewGIDocs] [bit] NULL,
	[DidYouReviewTwentyFourMonthChainOfTitle] [bit] NULL,
	[DidYouUploadPrelimAndSPToSmartView] [bit] NULL,
	[DidYouSendCompletionEmailToClient] [bit] NULL,
	[County] [nvarchar](300) NULL,
	[IsRushOrder] [bit] NOT NULL,
	[RushOrderComments] [varchar](1000) NULL,
	[UserAssigned] [int] NULL,
	[OrderMovementComments] [varchar](2000) NULL,
	[ReasonToAccept] [varchar](2000) NULL,
	[ReasonToAcceptCompletedOrder] [varchar](2000) NULL,
	[CWLTTitleOfficeName] [varchar](1000) NULL,
	[CWLTTitleOfficerName] [varchar](1000) NULL,
	[AnyPostingFoundInPIGI] [bit] NULL,
	[AmendmentDate] [datetime] NULL,
 CONSTRAINT [PK_Audits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditSenders]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditSenders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[EmailAddress] [varchar](200) NOT NULL,
	[CrnId] [int] NOT NULL,
	[AttachmentRequired] [bit] NOT NULL,
 CONSTRAINT [PK_AuditSenders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditStatuses]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_AuditStates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditTasks]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_AuditTasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditTimeEntries]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditTimeEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditId] [int] NOT NULL,
	[AuditTaskId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[IsInprogress] [bit] NOT NULL,
 CONSTRAINT [PK_AuditTimeEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditUpdates]    Script Date: 9/24/2026 9:10:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditUpdates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuditId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AuditTaskId] [int] NOT NULL,
	[Updates] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_AuditUpdates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditErrorTypes] ADD  CONSTRAINT [DF_AuditErrorTypes_IsCritical]  DEFAULT ((1)) FOR [IsCritical]
GO
ALTER TABLE [dbo].[Audits] ADD  CONSTRAINT [DF_Audits_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Audits] ADD  CONSTRAINT [DF_Audits_UserCreated]  DEFAULT ((1)) FOR [UserCreated]
GO
ALTER TABLE [dbo].[Audits] ADD  CONSTRAINT [Audit_IsRequired_default]  DEFAULT ((0)) FOR [IsQcRequired]
GO
ALTER TABLE [dbo].[Audits] ADD  DEFAULT ((0)) FOR [IsRushOrder]
GO
ALTER TABLE [dbo].[AuditSenders] ADD  CONSTRAINT [DF_AuditSenders_AttachmentRequired]  DEFAULT ((0)) FOR [AttachmentRequired]
GO
ALTER TABLE [dbo].[AuditTimeEntries] ADD  CONSTRAINT [DF_AuditTimeEntries_IsInprogress]  DEFAULT ((0)) FOR [IsInprogress]
GO
ALTER TABLE [dbo].[AuditCheckCrns]  WITH CHECK ADD  CONSTRAINT [FK_AuditCheckCrns_AuditChecks] FOREIGN KEY([AuditCheckId])
REFERENCES [dbo].[AuditChecks] ([Id])
GO
ALTER TABLE [dbo].[AuditCheckCrns] CHECK CONSTRAINT [FK_AuditCheckCrns_AuditChecks]
GO
ALTER TABLE [dbo].[AuditChecks]  WITH CHECK ADD  CONSTRAINT [FK_AuditChecks_CRN] FOREIGN KEY([OfficeId])
REFERENCES [dbo].[CRN] ([CRNID])
GO
ALTER TABLE [dbo].[AuditChecks] CHECK CONSTRAINT [FK_AuditChecks_CRN]
GO
ALTER TABLE [dbo].[AuditChecks]  WITH CHECK ADD  CONSTRAINT [FK_AuditChecks_TitleOrderTasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[TitleOrderTasks] ([Id])
GO
ALTER TABLE [dbo].[AuditChecks] CHECK CONSTRAINT [FK_AuditChecks_TitleOrderTasks]
GO
ALTER TABLE [dbo].[AuditCheckTasks]  WITH CHECK ADD  CONSTRAINT [FK_AuditCheckTasks_AuditChecks] FOREIGN KEY([AuditCheckId])
REFERENCES [dbo].[AuditChecks] ([Id])
GO
ALTER TABLE [dbo].[AuditCheckTasks] CHECK CONSTRAINT [FK_AuditCheckTasks_AuditChecks]
GO
ALTER TABLE [dbo].[AuditCheckValues]  WITH CHECK ADD  CONSTRAINT [FK_AuditCheckValues_AuditChecks] FOREIGN KEY([AuditCheckId])
REFERENCES [dbo].[AuditChecks] ([Id])
GO
ALTER TABLE [dbo].[AuditCheckValues] CHECK CONSTRAINT [FK_AuditCheckValues_AuditChecks]
GO
ALTER TABLE [dbo].[AuditDocuments]  WITH CHECK ADD  CONSTRAINT [FK_AuditDocuments_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditDocuments] CHECK CONSTRAINT [FK_AuditDocuments_Audits]
GO
ALTER TABLE [dbo].[AuditErrorJson]  WITH CHECK ADD  CONSTRAINT [FK_AuditErrorJson_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditErrorJson] CHECK CONSTRAINT [FK_AuditErrorJson_Audits]
GO
ALTER TABLE [dbo].[AuditErrorJson]  WITH CHECK ADD  CONSTRAINT [FK_AuditErrorJson_AuditTasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[AuditTasks] ([Id])
GO
ALTER TABLE [dbo].[AuditErrorJson] CHECK CONSTRAINT [FK_AuditErrorJson_AuditTasks]
GO
ALTER TABLE [dbo].[AuditErrorTypes]  WITH CHECK ADD  CONSTRAINT [FK_AuditErrorTypes_AuditErrorCategories] FOREIGN KEY([AuditErrorCategoryId])
REFERENCES [dbo].[AuditErrorCategories] ([Id])
GO
ALTER TABLE [dbo].[AuditErrorTypes] CHECK CONSTRAINT [FK_AuditErrorTypes_AuditErrorCategories]
GO
ALTER TABLE [dbo].[AuditHolds]  WITH CHECK ADD  CONSTRAINT [FK_AuditHolds_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditHolds] CHECK CONSTRAINT [FK_AuditHolds_Audits]
GO
ALTER TABLE [dbo].[AuditHolds]  WITH CHECK ADD  CONSTRAINT [FK_AuditHolds_AuditTasks] FOREIGN KEY([AuditTaskId])
REFERENCES [dbo].[AuditTasks] ([Id])
GO
ALTER TABLE [dbo].[AuditHolds] CHECK CONSTRAINT [FK_AuditHolds_AuditTasks]
GO
ALTER TABLE [dbo].[AuditHolds]  WITH CHECK ADD  CONSTRAINT [FK_AuditHolds_USERINFO] FOREIGN KEY([HoldBy])
REFERENCES [dbo].[USERINFO] ([USERID])
GO
ALTER TABLE [dbo].[AuditHolds] CHECK CONSTRAINT [FK_AuditHolds_USERINFO]
GO
ALTER TABLE [dbo].[AuditRejects]  WITH CHECK ADD  CONSTRAINT [FK_AuditRejects_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditRejects] CHECK CONSTRAINT [FK_AuditRejects_Audits]
GO
ALTER TABLE [dbo].[AuditRejects]  WITH CHECK ADD  CONSTRAINT [FK_AuditRejects_USERINFO] FOREIGN KEY([RejectedBy])
REFERENCES [dbo].[USERINFO] ([USERID])
GO
ALTER TABLE [dbo].[AuditRejects] CHECK CONSTRAINT [FK_AuditRejects_USERINFO]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_CRN] FOREIGN KEY([CrnId])
REFERENCES [dbo].[CRN] ([CRNID])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_CRN]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_AuditRequestTypes] FOREIGN KEY([RequestTypeId])
REFERENCES [dbo].[AuditRequestTypes] ([Id])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_AuditRequestTypes]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_AuditSenders] FOREIGN KEY([SenderId])
REFERENCES [dbo].[AuditSenders] ([Id])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_AuditSenders]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_AuditStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[AuditStatuses] ([Id])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_AuditStatuses]
GO
ALTER TABLE [dbo].[Audits]  WITH CHECK ADD  CONSTRAINT [FK_Audits_AuditTasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[AuditTasks] ([Id])
GO
ALTER TABLE [dbo].[Audits] CHECK CONSTRAINT [FK_Audits_AuditTasks]
GO
ALTER TABLE [dbo].[AuditSenders]  WITH CHECK ADD  CONSTRAINT [FK_AuditSenders_CRN] FOREIGN KEY([CrnId])
REFERENCES [dbo].[CRN] ([CRNID])
GO
ALTER TABLE [dbo].[AuditSenders] CHECK CONSTRAINT [FK_AuditSenders_CRN]
GO
ALTER TABLE [dbo].[AuditTimeEntries]  WITH CHECK ADD  CONSTRAINT [FK_AuditTimeEntries_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditTimeEntries] CHECK CONSTRAINT [FK_AuditTimeEntries_Audits]
GO
ALTER TABLE [dbo].[AuditTimeEntries]  WITH CHECK ADD  CONSTRAINT [FK_AuditTimeEntries_AuditTasks] FOREIGN KEY([AuditTaskId])
REFERENCES [dbo].[AuditTasks] ([Id])
GO
ALTER TABLE [dbo].[AuditTimeEntries] CHECK CONSTRAINT [FK_AuditTimeEntries_AuditTasks]
GO
ALTER TABLE [dbo].[AuditTimeEntries]  WITH CHECK ADD  CONSTRAINT [FK_AuditTimeEntries_AuditTimeEntries] FOREIGN KEY([Id])
REFERENCES [dbo].[AuditTimeEntries] ([Id])
GO
ALTER TABLE [dbo].[AuditTimeEntries] CHECK CONSTRAINT [FK_AuditTimeEntries_AuditTimeEntries]
GO
ALTER TABLE [dbo].[AuditTimeEntries]  WITH CHECK ADD  CONSTRAINT [FK_AuditTimeEntries_USERINFO] FOREIGN KEY([UserId])
REFERENCES [dbo].[USERINFO] ([USERID])
GO
ALTER TABLE [dbo].[AuditTimeEntries] CHECK CONSTRAINT [FK_AuditTimeEntries_USERINFO]
GO
ALTER TABLE [dbo].[AuditUpdates]  WITH CHECK ADD  CONSTRAINT [FK_AuditUpdates_Audits] FOREIGN KEY([AuditId])
REFERENCES [dbo].[Audits] ([Id])
GO
ALTER TABLE [dbo].[AuditUpdates] CHECK CONSTRAINT [FK_AuditUpdates_Audits]
GO
ALTER TABLE [dbo].[AuditUpdates]  WITH CHECK ADD  CONSTRAINT [FK_AuditUpdates_AuditTasks] FOREIGN KEY([AuditTaskId])
REFERENCES [dbo].[AuditTasks] ([Id])
GO
ALTER TABLE [dbo].[AuditUpdates] CHECK CONSTRAINT [FK_AuditUpdates_AuditTasks]
GO
ALTER TABLE [dbo].[AuditUpdates]  WITH CHECK ADD  CONSTRAINT [FK_AuditUpdates_USERINFO] FOREIGN KEY([UserId])
REFERENCES [dbo].[USERINFO] ([USERID])
GO
ALTER TABLE [dbo].[AuditUpdates] CHECK CONSTRAINT [FK_AuditUpdates_USERINFO]
GO

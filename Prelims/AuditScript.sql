
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

----------------


GO
SET IDENTITY_INSERT [dbo].[AuditErrorCategories] ON 
GO
INSERT [dbo].[AuditErrorCategories] ([Id], [Name]) VALUES (1, N'Other')
GO
SET IDENTITY_INSERT [dbo].[AuditErrorCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditErrorTypes] ON 
GO
INSERT [dbo].[AuditErrorTypes] ([Id], [Name], [AuditErrorCategoryId], [IsCritical]) VALUES (1, N'Other', 1, 1)
GO
SET IDENTITY_INSERT [dbo].[AuditErrorTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditHoldReasons] ON 
GO
INSERT [dbo].[AuditHoldReasons] ([Id], [Name]) VALUES (5, N'Tool locked')
GO
INSERT [dbo].[AuditHoldReasons] ([Id], [Name]) VALUES (6, N'Not migrated')
GO
SET IDENTITY_INSERT [dbo].[AuditHoldReasons] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditRejectReasons] ON 
GO
INSERT [dbo].[AuditRejectReasons] ([Id], [Name]) VALUES (6, N'Repeated order')
GO
INSERT [dbo].[AuditRejectReasons] ([Id], [Name]) VALUES (7, N'Unable to access in Title Point')
GO
INSERT [dbo].[AuditRejectReasons] ([Id], [Name]) VALUES (8, N'As per client instruction')
GO
SET IDENTITY_INSERT [dbo].[AuditRejectReasons] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditRequestTypes] ON 
GO
INSERT [dbo].[AuditRequestTypes] ([Id], [Name]) VALUES (1, N'Refi')
GO
INSERT [dbo].[AuditRequestTypes] ([Id], [Name]) VALUES (2, N'Resale')
GO
INSERT [dbo].[AuditRequestTypes] ([Id], [Name]) VALUES (4, N'Other')
GO
SET IDENTITY_INSERT [dbo].[AuditRequestTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditSenders] ON 
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1007, N'Julio Valencia', N'Julio.Valencia@fnf.com', 21, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1008, N'Jim Park', N'jim.park@fnf.com', 21, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1014, N'Jill Culver', N'Team.Jill@fnf.com', 21, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1023, N'Steven', N'steven.lombardo@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1024, N'Sindy', N'team.jill@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1025, N'Tony', N'team.jill@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1026, N'Jill', N'team.jill@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1027, N'Team', N'team.steve@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1028, N'Jim', N'jim.park@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1029, N'Julio', N'team.sheila@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1030, N'Tim', N'team.sheila@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1031, N'Debby', N'team.sheila@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1032, N'Ruben ', N'team.ruben@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1033, N'Cheryl', N'cvajnar@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1034, N'Adam', N'adam.markland@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1035, N'Lisa', N'lisa.kreueger@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1036, N'Mariel', N'mariel.moore@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1037, N'Cesar', N'team.cesar@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1038, N'Manuel', N'team.cesar@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1039, N'Cesar', N'cesar.hernandez@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1040, N'Manuel', N'manuel.pedroza@fnf.com', 21, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1041, N'Mark', N'mark.mcdonald@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1042, N'Stephanie', N'stephanie.fields@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1043, N'Vicky', N'vicky.ezzell@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1044, N'Alexandra', N'vickysteam@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1045, N'Jennifer', N'vickysteam@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1046, N'Jennifer', N'jennifer.white@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1047, N'Tina', N'tgagnon@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1048, N'Margaret', N'traceysteam@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1049, N'Tracey', N'tculley-rojas@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1050, N'Alexandra', N'alexandra.esparza@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1051, N'Margaret', N'margaret.sandoval@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1052, N'Tina', N'vickysteam@fnf.com', 22, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1053, N'Team', N'mcopenorders@fnf.com', 26, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1054, N'Justine', N'justinesteam@fnf.com', 26, 1)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1055, N'RODEL ESQUEJO', N'equejor@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1056, N'Mellany DeLeon', N'ldeleon@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1057, N'Joemar Reyes', N'joemar.reyes@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1058, N'Ralph Abrego', N'Abrgegor@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1059, N'Rosa Medrano', N'Rmedrano@cltic.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1060, N'Chris Maziar', N'cmaziar@cltic.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1061, N'Adriana Santillan', N'adriana.santillan@cltic.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1062, N'Eric Gile', N'TeamGile@cltic.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1063, N'Babak Alihaji', N'bobby@allcaltitle.com', 36, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1064, N'Veronica Loaiza-Concetti', N'TeamConcetti@monarchtc.com', 38, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1065, N'Joes', N'joesteam@fnf.com', 22, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1066, N'Albert', N'team.albert@fnf.com', 21, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1067, N'Kevin Young', N'kyoung@allcaltitle.com', 36, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1068, N'David Noble', N'david.noble@fnf.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1069, N'Maria Soto', N'Maria.Soto1@fnf.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1070, N'Team Jay', N'TeamJay@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1071, N'Glendale Open Order', N'OpenOrder.Glendale@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1072, N'Paul Gonzalez', N'Paul.Gonzalez@fnf.com', 22, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1073, N'Team Julio', N'team.julio@fnf.com', 22, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1074, N'Title Team', N'team@allcaltitle.com', 36, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1076, N'Chris Otten', N'chris.otten@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1077, N'Lisa Allen', N'lisa.allen@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1078, N'Team Tayco', N'Team.tayco@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1079, N'ramakrishna', N'rama@firstfocusbpo.com', 42, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1080, N'Lee Wilson', N'Lee.Wilson@fnf.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1081, N'David Neal', N'David.Neal@fnf.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1082, N'Ryan McMahon', N'ryan@allcaltitle.com', 36, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1083, N'Patrick Meza', N'Patrick.Meza@ltic.com', 39, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1084, N'Dexter  Batanga', N'Dexter.Batanga@ltic.com', 39, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1085, N'Anthony Marquez', N'Anthony@monarchtc.com', 38, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1086, N'Joemar Reyes', N'joemar.reyes@ctt.com', 34, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1087, N'inhouseupdate', N'inhouseupdate@ltic.com', 39, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1088, N'Jeff Martin', N'Jeff.Martin@ctt.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1089, N'Ryan Telle', N'Ryan.Telle@ctt.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1090, N'Sheila Isham', N'Sheila.Isham@fnf.com', 21, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1091, N'Jose', N'JoesTeam@fnf.com', 40, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1092, N'Michelle Pascual', N'michelle.pascual@ticortitle.com', 43, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1093, N'Bailey Dela Cruz', N'Bailey.DelaCruz@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1094, N'Claudia Torres', N'Claudia.Torres@ctt.com', 33, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1095, N'Ana Regalado', N'ana.regalado@cltic.com', 32, 0)
GO
INSERT [dbo].[AuditSenders] ([Id], [Name], [EmailAddress], [CrnId], [AttachmentRequired]) VALUES (1096, N'RMB-Team.CPFSocal', N'Team.CPFSoCal@fnf.com', 21, 0)
GO
SET IDENTITY_INSERT [dbo].[AuditSenders] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditStatuses] ON 
GO
INSERT [dbo].[AuditStatuses] ([Id], [Name]) VALUES (1, N'Pending')
GO
INSERT [dbo].[AuditStatuses] ([Id], [Name]) VALUES (2, N'Inprogress')
GO
INSERT [dbo].[AuditStatuses] ([Id], [Name]) VALUES (3, N'Hold')
GO
INSERT [dbo].[AuditStatuses] ([Id], [Name]) VALUES (4, N'Completed')
GO
INSERT [dbo].[AuditStatuses] ([Id], [Name]) VALUES (5, N'Reject')
GO
SET IDENTITY_INSERT [dbo].[AuditStatuses] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditTasks] ON 
GO
INSERT [dbo].[AuditTasks] ([Id], [Name]) VALUES (1, N'L&V')
GO
INSERT [dbo].[AuditTasks] ([Id], [Name]) VALUES (2, N'PI')
GO
INSERT [dbo].[AuditTasks] ([Id], [Name]) VALUES (3, N'GI')

GO
INSERT [dbo].[AuditTasks] ([Id], [Name]) VALUES (4, N'Starter')
GO
INSERT [dbo].[AuditTasks] ([Id], [Name]) VALUES (5, N'Notes')

SET IDENTITY_INSERT [dbo].[AuditTasks] OFF
GO
-----26-09-2026----
ALTER TABLE dbo.audits
ADD TitleOrderId INT;

INSERT [dbo].[AuditTasks] ([Name]) VALUES (N'Taxes')
-------------------
-- Seed Dynamic Checks from Excel for Audit Process
-- Tasks:
--   TaskId 6: Taxes
--   TaskId 1: L&V
--   TaskId 2: PI
--   TaskId 3: GI
--   TaskId 4: Starter

BEGIN TRANSACTION;

-- Clean existing audit checks
delete from [dbo].[AuditCheckValues];
delete from [dbo].[AuditCheckTasks];
delete from [dbo].[AuditCheckCrns];
delete from[dbo].[AuditChecks];

DECLARE @CheckId INT;

-- =====================================================================
-- 1. TAXES (TaskId = 6)
-- =====================================================================
INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Refer with Order Sheet (APN and Address)', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 6);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Both Property Address in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 6);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check APN Map Marking in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 6);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check All Tax shown in Prelim with correct code', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 6);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Amount and Typo in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 6);

-- =====================================================================
-- 2. L&V (TaskId = 1)
-- =====================================================================
INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Refer with Order Sheet and Tax Sheet (Owner Name)', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Effective Date', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check All Deed in Title Point or Data Trace and Retrive', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Transaction and check typos in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check 24 month code and check typos in prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Vesting same Trust and LLC etc codes and typo in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Fee Type', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Policy Type', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 1);

-- =====================================================================
-- 3. PI (TaskId = 2)
-- =====================================================================
INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Refer with Order Sheet and Tax Sheet (Owner Name)', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 2);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check All Money Matters in Title Point or Data Trace and Retrive', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 2);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Retrive DOT and all document Check codes and check typos in prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 2);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'No Open DOT Run Grantor and Grantee search', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 2);

-- =====================================================================
-- 4. GI (TaskId = 3)
-- =====================================================================
INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Refer with Order Sheet (Buyer and Seller names)', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check all deed and PI documents for name search', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check all names Direct hit and Possible hit', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check All Tax shown in Prelim with correct code', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Amount and Typo in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check all GI matters Typo in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'LLC Entity name search', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Grantor and Grantee search', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 3);

-- =====================================================================
-- 5. STARTER (TaskId = 4)
-- =====================================================================
INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Refer with PIQ Starter and AP Starter', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 4);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check PI in Title Point or Data Trace for starter documents', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 4);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check all codes and typos in Prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 4);

INSERT INTO [dbo].[AuditChecks] ([Name], [AuditCheckValues]) VALUES (N'Check Chronological order entire prelim', N'Yes|No');
SET @CheckId = SCOPE_IDENTITY();
INSERT INTO [dbo].[AuditCheckTasks] ([AuditCheckId], [TaskId]) VALUES (@CheckId, 4);

COMMIT TRANSACTION;


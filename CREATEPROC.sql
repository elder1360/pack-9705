
USE AdventureWorks---------------------->DONT FORGET TO CHANGE THE DATABASE!!
GO
DECLARE @PLAN CHAR(7)
SET @PLAN=''---------------------------->TYPE 'EXECUTE' HERE IF YOU WANT TO ACTUALLY CREATE THE PROCEDURES IN YOUR DATABASE...
											--  OR IT ONLY PRINTS THE PROCEDURES HERE


------------------------------------------JUNK TABLE DETECTION PHASE
SET NOCOUNT ON;
DECLARE @JunkTable TABLE
(
	Id INT IDENTITY UNIQUE,
	JunkTableName NVARCHAR(50)
);
DECLARE @JtableName NVARCHAR(100),@JcolumnName NVARCHAR(100),@JConstraintName NVARCHAR(100),
@JColumnDefault NVARCHAR(100),@IsJunk BIT;

DECLARE JunkTableDetection_TablesCursor Cursor
FOR
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='BASE TABLE';

OPEN JunkTableDetection_TablesCursor

FETCH NEXT FROM JunkTableDetection_TablesCursor
INTO @JtableName;

DECLARE JunkTableDetection_ColumnCursor CURSOR
FOR
SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME=@JtableName

OPEN JunkTableDetection_ColumnCursor

WHILE @@FETCH_STATUS=0
BEGIN

	FETCH NEXT FROM JunkTableDetection_ColumnCursor
	INTO @JcolumnName,@JColumnDefault

	BEGIN TRY
	SET @JConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
	WHERE TABLE_NAME=@JtableName AND COLUMN_NAME=@JcolumnName);
	IF @JColumnDefault IS NULL
		SET @IsJunk=0;
	END TRY
	BEGIN CATCH
		SET @IsJunk=1;			
	END CATCH
END

IF @IsJunk=1
	BEGIN
		INSERT INTO @JunkTable VALUES(@JtableName)
	END

CLOSE JunkTableDetection_ColumnCursor
DEALLOCATE JunkTableDetection_ColumnCursor

FETCH NEXT FROM JunkTableDetection_TablesCursor
INTO @JtableName;

WHILE @@FETCH_STATUS=0
BEGIN
	DECLARE JunkTableDetection_ColumnCursor CURSOR
	FOR
	SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME=@JtableName

	OPEN JunkTableDetection_ColumnCursor

	SET @IsJunk=0;
	WHILE @@FETCH_STATUS=0
	BEGIN
		
		FETCH NEXT FROM JunkTableDetection_ColumnCursor
		INTO @JcolumnName,@JColumnDefault;
			
		BEGIN TRY
		SET @JConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
		WHERE TABLE_NAME=@JtableName AND COLUMN_NAME=@JcolumnName);
		IF @JColumnDefault IS NULL
			SET @IsJunk=0;
		END TRY
		BEGIN CATCH
			SET @IsJunk=1			
		END CATCH

		FETCH NEXT FROM JunkTableDetection_ColumnCursor
		INTO @JcolumnName,@JColumnDefault;		
		
	END

	IF @IsJunk=1
		BEGIN
			INSERT INTO @JunkTable VALUES(@JtableName)
		END

	CLOSE JunkTableDetection_ColumnCursor
	DEALLOCATE JunkTableDetection_ColumnCursor

	FETCH NEXT FROM JunkTableDetection_TablesCursor
	INTO @JtableName;
	
	END

	CLOSE JunkTableDetection_TablesCursor
	DEALLOCATE JunkTableDetection_TablesCursor
	SET NOCOUNT OFF;
------------------------------------------END OF JUNK TABLE DETECTION PHASE

------------------------------------------DECLARE NEEDED VARIABLES

	DECLARE @TableSchema NVARCHAR(50),@TableName NVARCHAR(50),
			@TableType NVARCHAR(20),@ColumnName NVARCHAR(50),
			@DataType NVARCHAR(20),@CharacterMaxLenght NVARCHAR(10),
			@ColumnDefault NVARCHAR(20),@OrdinalPosition TINYINT;

------------------------------------------DECLARE TABLES CURSOR

	DECLARE MyAdventureTables_Cursor CURSOR
	FOR
	SELECT TABLE_SCHEMA,TABLE_NAME
	FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT IN (SELECT JunkTableName FROM @JunkTable)

------------------------------------------OPEN TABLE CURSOR
	OPEN MyAdventureTables_Cursor;

------------------------------------------FETCH FIRST ROW OF TABLE CURSOR

FETCH NEXT FROM MyAdventureTables_Cursor
	INTO @TableSchema,@TableName;
	WHILE (SELECT temporal_type
	FROM   sys.tables
	WHERE  object_id = OBJECT_ID(@TableSchema+'.'+@TableName, 'u'))=1
		BEGIN 
				FETCH NEXT FROM MyAdventureTables_Cursor
				INTO @TableSchema,@TableName;
		END

------------------------------------------DECLARE COLUMN CURSOR

	DECLARE MyAdventureColumns_Cursor CURSOR
		FOR
		SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,
		COLUMN_DEFAULT,ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@TableName
		ORDER BY ORDINAL_POSITION

------------------------------------------OPEN COLUMN CURSOR

	OPEN MyAdventureColumns_Cursor;

------------------------------------------FETCH FIRST ROW OF COLUMN CURSOR

	FETCH NEXT FROM MyAdventureColumns_Cursor
		INTO @ColumnName,@DataType,@CharacterMaxLenght,
		@ColumnDefault,@OrdinalPosition;

------------------------------------------DECLARE AND SET VARIABLES NEEDED FOR PREPARING THE PROC TEXT

	DECLARE @PROC_INSERT NVARCHAR(2500),@ColumnNames NVARCHAR(1000),
	@Values NVARCHAR(1000),@HasConstraint BIT,@PROC_DELETE NVARCHAR(2500),
	@PROC_UPDATE NVARCHAR(2500),@PROC_UPDATE_SET NVARCHAR(1000),
	@PKCOLUMN NVARCHAR(50),@ConstraintName NVARCHAR(200),
	@PROC_SELECT NVARCHAR(2500),@DELETE_DONE BIT,@SELECT_DONE BIT;
	SET @PROC_INSERT='';
	SET @ColumnNames='';
	SET @Values='';
	SET @HasConstraint=0;
	SET @PROC_DELETE='';
	SET @PROC_UPDATE='';
	SET @PKCOLUMN='';
	SET @PROC_UPDATE_SET='';
	SET @ConstraintName='';
	SET @PROC_SELECT='';
	SET @DELETE_DONE=0;
	SET @SELECT_DONE=0;
	SET @HasConstraint=0;

------------------------------------------INSERT OPERATIONS FIRST LINE
			
	SET @PROC_INSERT=N'CREATE PROC ['+@TableSchema+'].'+@TableName+'_INSERT (';

------------------------------------------UPDATE OPERATIONS FIRST LINE
		
	SET @PROC_UPDATE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_UPDATE ('

------------------------------------------LOOP IN COLUMNS
	WHILE @@FETCH_STATUS=0--while for first table and loop through to end of its columns
	BEGIN--begin of first loop through columns of first table

------------------------------------------INSERT OPERATIONS
		IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsComputed'))=0 AND 
		(SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'GeneratedAlwaysType'))=0
		BEGIN
			SET @ConstraintName='';			
			BEGIN TRY
			SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
			WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
				IF @ConstraintName LIKE N'FK%' OR @ConstraintName LIKE N'PK%'
					BEGIN
						SET @HasConstraint=1;
					END
				ELSE
					BEGIN
						IF @ColumnDefault IS NULL
							BEGIN
								IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
									BEGIN
										SET @PROC_INSERT=@PROC_INSERT+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+N',';
										SET @ColumnNames=@ColumnNames+'['+@ColumnName+'],';
									END
								ELSE
									BEGIN
										IF @CharacterMaxLenght<0
											SET @CharacterMaxLenght=1
										SET @PROC_INSERT=@PROC_INSERT+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+')'+N',';
										SET @ColumnNames=@ColumnNames+'['+@ColumnName+'],';
									END
							END
					END
			END TRY		

			BEGIN CATCH
				SET @HasConstraint=1;
			END CATCH
	END
------------------------------------------DELETE OPERATIONS
		IF @DELETE_DONE=0
		BEGIN
			BEGIN TRY
				SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
				IF @ConstraintName LIKE 'PK%'
					BEGIN
						IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
							BEGIN
								SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
								+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
END
	';
							END
						ELSE
							BEGIN
								SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
								+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
END
	';
							END
					END
				SET @DELETE_DONE=1;
				END TRY

				BEGIN CATCH
				IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
				BEGIN
					SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
								+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
END
	';
				END

				ELSE
				BEGIN
					IF @CharacterMaxLenght<0
						SET @CharacterMaxLenght=1
						SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
								+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
END
	';
				END
				SET @DELETE_DONE=1;
				END CATCH
		END

------------------------------------------UPDATE OPERATIONS
		
		IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsComputed'))=0 AND
			(SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'GeneratedAlwaysType'))=0
			BEGIN
				SET @ConstraintName='';
				BEGIN TRY
				SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
				IF @ConstraintName LIKE '%PK%'
					BEGIN
						SET @PKCOLUMN=@ColumnName;
						SET @HasConstraint=1;
					END
				IF @ConstraintName LIKE '%FK%'
					BEGIN
						SET @HasConstraint=1;
					END
				ELSE
				BEGIN
					IF @ColumnDefault IS NULL
						BEGIN
							IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
								BEGIN	
									SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+',';
									IF @ConstraintName LIKE '%PK%'
										SET @HasConstraint+=1;
									ELSE
										SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
								END
							ELSE
								BEGIN
									IF @CharacterMaxLenght<0
										SET @CharacterMaxLenght=1
									SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'),';
									IF @ConstraintName LIKE '%PK%'
										SET @HasConstraint+=1;
									ELSE
										SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
								END
						END
				END
				END TRY

				BEGIN CATCH
				SET @HasConstraint=1;
				IF @PKCOLUMN=''
					SET @PKCOLUMN=@ColumnName;
				DECLARE @IsIdentity BIT;
				IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsIdentity'))=1
					SET @IsIdentity=1;
				IF @IsIdentity=1
					BEGIN
						SET @PKCOLUMN=@ColumnName;
					END
				ELSE
					BEGIN
						IF @ColumnDefault IS NULL
							BEGIN
								IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
									BEGIN	
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+',';
										IF @PKCOLUMN!=@ColumnName
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
								ELSE
									BEGIN
										IF @CharacterMaxLenght<0
											SET @CharacterMaxLenght=1
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'),';
										IF @PKCOLUMN!=@ColumnName
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
							END
					END

				END CATCH
				SET @IsIdentity=0;
			END
------------------------------------------SELECT OPERATIONS
		IF @SELECT_DONE=0
		BEGIN
			BEGIN TRY
				SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
				IF @OrdinalPosition=1 AND @ConstraintName LIKE 'PK%'
					BEGIN
						IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
							BEGIN 
								SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
AS
BEGIN
SELECT * FROM '+@TableName+'
WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
END'
							END
						ELSE
							BEGIN
								IF @CharacterMaxLenght<0
									SET @CharacterMaxLenght=1
								SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
AS
BEGIN
	SELECT * FROM '+@TableName+'
	WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
END'
							END
					END
			SET @SELECT_DONE=1;
			END TRY

			BEGIN CATCH
				IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
					BEGIN 
						SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
AS
BEGIN
	SELECT * FROM '+@TableName+'
	WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
END'
					END
				ELSE
					BEGIN
						SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
AS
BEGIN
	SELECT * FROM '+@TableName+'
	WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
END'
					END
			SET @SELECT_DONE=1;
			END CATCH
		END

------------------------------------------
	FETCH NEXT FROM MyAdventureColumns_Cursor
	INTO @ColumnName,@DataType,@CharacterMaxLenght
	,@ColumnDefault,@OrdinalPosition;

------------------------------------------
						
END--WHILE END of first loop through first table
	IF @HasConstraint=0
		SET @PROC_UPDATE=''

------------------------------------------UPDATE OPERATIONS COMPLETE

IF @PROC_UPDATE!=''
	BEGIN
		SET @PROC_UPDATE_SET=STUFF(@PROC_UPDATE_SET,LEN(@PROC_UPDATE_SET),1,'')+'
		';
		SET @PROC_UPDATE=STUFF(@PROC_UPDATE,LEN(@PROC_UPDATE),1,'')
		SET @PROC_UPDATE=COALESCE(@PROC_UPDATE,'') +')
		AS
		BEGIN
			SET NOCOUNT ON;
			UPDATE ['+@TableSchema+'].'+@TableName+' SET
			'+@PROC_UPDATE_SET+
				'WHERE '+@PKCOLUMN+'=@'+@PKCOLUMN+'
		END

		'
	END

------------------------------------------INSERT OPERATIONS COMPLETE
IF @PROC_INSERT!=''
	BEGIN	
		SET @ColumnNames=STUFF(@ColumnNames,LEN(@ColumnNames),1,'');
		SET @Values=REPLACE(@ColumnNames,' ','');
		SET @Values=REPLACE(@Values,'[','');
		SET @Values=REPLACE(@Values,']','');
		SET @Values=REPLACE(@Values,',',',@');
		SET @PROC_INSERT=STUFF(@PROC_INSERT,LEN(@PROC_INSERT),2,')');
		SET @PROC_INSERT=@PROC_INSERT+'
		AS
		BEGIN
			SET NOCOUNT ON;
			INSERT INTO ['+@TableSchema+'].['+@TableName+'] ('+@ColumnNames+')
			VALUES (@'+@Values+')
			RETURN SCOPE_IDENTITY()
		END
	
				'
	END
------------------------------------------PRINT OR EXECUTE

IF @PLAN='EXECUTE'
	BEGIN
		EXEC sp_executesql @PROC_INSERT;
		EXEC sp_executesql @PROC_DELETE;
		EXEC sp_executesql @PROC_UPDATE;
		EXEC sp_executesql @PROC_SELECT;
	END
ELSE
	BEGIN
		PRINT '-------------INSERT PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_INSERT
		PRINT '-------------DELETE PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_DELETE
		PRINT '-------------UPDATE PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_UPDATE
		PRINT '-------------SELECT PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_SELECT
	END

------------------------------------------CLOSE AND DEALLOCATE COLUMN CURSOR

CLOSE MyAdventureColumns_Cursor
DEALLOCATE MyAdventureColumns_Cursor

------------------------------------------FETCH SECOND ROW OF TABLE CURSOR

FETCH NEXT FROM MyAdventureTables_Cursor
	INTO @TableSchema,@TableName;
	WHILE (SELECT temporal_type
	FROM   sys.tables
	WHERE  object_id = OBJECT_ID(@TableSchema+'.'+@TableName, 'u'))=1
		BEGIN 
				FETCH NEXT FROM MyAdventureTables_Cursor
				INTO @TableSchema,@TableName;
		END

------------------------------------------LOOP THROUGH TABLE CURSOR

WHILE @@FETCH_STATUS=0
BEGIN--begin of loop through all tables to end
------------------------------------------RESETTING THE VARIABLES NEEDED FOR PROC OPERATION

	SET @HasConstraint=@HasConstraint+1
	SET @PROC_INSERT='';
	SET @ColumnNames='';
	SET @Values='';
	SET @PROC_DELETE='';
	SET @PKCOLUMN='';
	SET @PROC_UPDATE_SET='';
	SET @PROC_UPDATE='';
	SET @DELETE_DONE=0;
	SET @SELECT_DONE=0;
	SET @HasConstraint=0;

------------------------------------------DECLARE COLUMN CURSOR WITHIN THE TABLES LOOP

	DECLARE MyAdventureColumns_Cursor CURSOR
	FOR
	SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,COLUMN_DEFAULT,ORDINAL_POSITION
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME=@TableName
	ORDER BY ORDINAL_POSITION;

------------------------------------------OPEN COLUMN CURSOR

	OPEN MyAdventureColumns_Cursor;

------------------------------------------INSERT OPERATIONS FIRST LINE
			
	SET @PROC_INSERT=N'CREATE PROC ['+@TableSchema+'].'+@TableName+'_INSERT (';

------------------------------------------UPDATE OPERATIONS FIRST LINE
		
	SET @PROC_UPDATE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_UPDATE ('


------------------------------------------FETCH NEXT COLUMN FROM COLUMN CURSOR
		FETCH NEXT FROM MyAdventureColumns_Cursor
		INTO @ColumnName,@DataType,@CharacterMaxLenght,@ColumnDefault,@OrdinalPosition;

------------------------------------------LOOP THROUGH COLUMN CURSOR

	WHILE @@FETCH_STATUS=0
	BEGIN--begin of loop throgh next batch of columns in all tables to end

------------------------------------------INSERT OPERATIONS
		IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsComputed'))=0 AND
		(SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'GeneratedAlwaysType'))=0
			BEGIN
				SET @ConstraintName='';			
				BEGIN TRY
				SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
					IF @ConstraintName LIKE N'FK%' OR @ConstraintName LIKE N'PK%'
						BEGIN
							SET @HasConstraint=1;
						END
					ELSE
						BEGIN
							IF @ColumnDefault IS NULL
								BEGIN
									IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
										BEGIN
											SET @PROC_INSERT=@PROC_INSERT+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+N',';
											SET @ColumnNames=@ColumnNames+'['+@ColumnName+'],';
										END
									ELSE
										BEGIN
											IF @CharacterMaxLenght<0
												SET @CharacterMaxLenght=1
											SET @PROC_INSERT=@PROC_INSERT+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+')'+N',';
											SET @ColumnNames=@ColumnNames+'['+@ColumnName+'],';
										END
								END
						END
				END TRY

				BEGIN CATCH
					SET @HasConstraint=1;
				END CATCH
			END
		--IF @HasConstraint=0
		--	SET @PROC_INSERT='';
------------------------------------------DELETE OPERATIONS
		IF @DELETE_DONE=0
			BEGIN
				BEGIN TRY
					SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
					WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
					IF @ConstraintName LIKE '%PK%'
						BEGIN
							IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
								BEGIN
									SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
									+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
	AS
	BEGIN
		SET NOCOUNT ON;
		DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
	END
		';
								END
							ELSE
								BEGIN
									SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
									+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
	AS
	BEGIN
		SET NOCOUNT ON;
		DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
	END
		';
								END
						END
					IF @PROC_DELETE!=''
						SET @DELETE_DONE=1;
					END TRY

					BEGIN CATCH
					IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
					BEGIN
						SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
									+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
	AS
	BEGIN
		SET NOCOUNT ON;
		DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
	END
		';
					END

					ELSE
					BEGIN
						IF @CharacterMaxLenght<0
							SET @CharacterMaxLenght=1
							SET @PROC_DELETE='CREATE PROC ['+@TableSchema+'].'+@TableName+'_DELETE (@'
									+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
	AS
	BEGIN
		SET NOCOUNT ON;
		DELETE FROM ['+@TableSchema+'].'+@TableName+' WHERE '+@ColumnName+' = @'+REPLACE(@ColumnName,' ','')+'
	END
		';
					END
					IF @PROC_DELETE!=''
						SET @DELETE_DONE=1;
					END CATCH
			END
------------------------------------------UPDATE OPERATIONS
		
		IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsComputed'))=0 AND
		(SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'GeneratedAlwaysType'))=0
			BEGIN
				SET @ConstraintName='';
				BEGIN TRY
				SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
				IF @ConstraintName LIKE '%PK%'
					BEGIN
						SET @PKCOLUMN=@ColumnName;
						SET @HasConstraint=1;
					END
				IF @ConstraintName LIKE '%FK%'
					BEGIN
						SET @HasConstraint=1;
					END
				ELSE
					BEGIN
						IF @ColumnDefault IS NULL
							BEGIN
								IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
									BEGIN	
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+',';
										IF @ConstraintName LIKE '%PK%'
											SET @HasConstraint+=1;
										ELSE
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
								ELSE
									BEGIN
										IF @CharacterMaxLenght<0
											SET @CharacterMaxLenght=1
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'),';
										IF @ConstraintName LIKE '%PK%'
											SET @HasConstraint+=1;
										ELSE
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
							END
					END
				END TRY

				BEGIN CATCH
				SET @HasConstraint=1;
				IF @PKCOLUMN=''
					SET @PKCOLUMN=@ColumnName;
				IF (SELECT COLUMNPROPERTY(object_id(@TableSchema+'.'+@TableName),@ColumnName,'IsIdentity'))=1
					SET @IsIdentity=1;
				IF @IsIdentity=1
					BEGIN
						SET @PKCOLUMN=@ColumnName;
					END
				ELSE
					BEGIN
						IF @ColumnDefault IS NULL
							BEGIN
								IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
									BEGIN	
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+',';
										IF @PKCOLUMN!=@ColumnName
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
								ELSE
									BEGIN
										IF @CharacterMaxLenght<0
											SET @CharacterMaxLenght=1
										SET @PROC_UPDATE=@PROC_UPDATE+'@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'),';
										IF @PKCOLUMN!=@ColumnName
											SET @PROC_UPDATE_SET=COALESCE(@PROC_UPDATE_SET,'')+'['+@ColumnName+']'+'=@'+REPLACE(@ColumnName,' ','')+',';
									END
							END
					END

				END CATCH
				SET @IsIdentity=0;
			END
------------------------------------------SELECT OPERATIONS
		IF @SELECT_DONE=0
			BEGIN
				BEGIN TRY
					SET @ConstraintName=(SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
					WHERE TABLE_NAME=@TableName AND COLUMN_NAME=@ColumnName);
					IF @OrdinalPosition=1 AND @ConstraintName LIKE 'PK%'
						BEGIN
							IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
								BEGIN 
									SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
	AS
	BEGIN
	SELECT * FROM '+@TableName+'
	WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
	END'
								END
							ELSE
								BEGIN
									IF @CharacterMaxLenght<0
										SET @CharacterMaxLenght=1
									SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
	AS
	BEGIN
		SELECT * FROM '+@TableName+'
		WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
	END'
								END
						END
				SET @SELECT_DONE=1;
				END TRY

				BEGIN CATCH
					IF @CharacterMaxLenght IS NULL OR @DataType='XML' OR @DataType='GEOGRAPHY' OR @DataType='HIERARCHYID'
						BEGIN 
							SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+')
	AS
	BEGIN
		SELECT * FROM '+@TableName+'
		WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
	END'
						END
					ELSE
						BEGIN
							SET @PROC_SELECT='CREATE PROC ['+@TableSchema+'].'+@TableName+'_SELECT (@'+REPLACE(@ColumnName,' ','')+' '+UPPER(@DataType)+'('+@CharacterMaxLenght+'))
	AS
	BEGIN
		SELECT * FROM '+@TableName+'
		WHERE '+@ColumnName+'=@'+REPLACE(@ColumnName,' ','')+'
	END'
						END
				SET @SELECT_DONE=1;
				END CATCH
			END
------------------------------------------FETCH NEXT COLUMN FROM COLUMN CURSOR
		FETCH NEXT FROM MyAdventureColumns_Cursor
		INTO @ColumnName,@DataType,@CharacterMaxLenght,@ColumnDefault,@OrdinalPosition;

------------------------------------------end of loop through next batch of columns in tables
						
	END
			IF @HasConstraint=0
			SET @PROC_UPDATE='';

------------------------------------------UPDATE OPERATIONS COMPLETE
		IF @PROC_UPDATE!=''
			BEGIN		
				SET @PROC_UPDATE_SET=STUFF(@PROC_UPDATE_SET,LEN(@PROC_UPDATE_SET),1,'')+'
		';
				SET @PROC_UPDATE=STUFF(@PROC_UPDATE,LEN(@PROC_UPDATE),1,'')
				SET @PROC_UPDATE=COALESCE(@PROC_UPDATE,'') +')
		AS
		BEGIN
			SET NOCOUNT ON;
			UPDATE ['+@TableSchema+'].'+@TableName+' SET
			'+@PROC_UPDATE_SET+
				'WHERE '+@PKCOLUMN+'=@'+@PKCOLUMN+'
		END

		'
			END
------------------------------------------INSERT OPERATIONS COMPLETE
		IF @PROC_INSERT!=''
			BEGIN
				SET @ColumnNames=STUFF(@ColumnNames,LEN(@ColumnNames),1,'');
				SET @Values=REPLACE(@ColumnNames,' ','');
				SET @Values=REPLACE(@Values,'[','');
				SET @Values=REPLACE(@Values,']','');
				SET @Values=REPLACE(@Values,',',',@');
				SET @PROC_INSERT=STUFF(@PROC_INSERT,LEN(@PROC_INSERT),2,')');
				SET @PROC_INSERT=@PROC_INSERT+'
		AS
		BEGIN
			SET NOCOUNT ON;
			INSERT INTO ['+@TableSchema+'].['+@TableName+'] ('+@ColumnNames+')
			VALUES (@'+@Values+')
			RETURN SCOPE_IDENTITY()
		END
	
				'
			END

------------------------------------------PRINT OR EXECUTE

IF @PLAN='EXECUTE'
	BEGIN
		EXEC sp_executesql @PROC_INSERT;
		EXEC sp_executesql @PROC_DELETE;
		EXEC sp_executesql @PROC_UPDATE;
		EXEC sp_executesql @PROC_SELECT;
	END
ELSE
	BEGIN
		PRINT '-------------INSERT PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_INSERT
		PRINT '-------------DELETE PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_DELETE
		PRINT '-------------UPDATE PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_UPDATE
		PRINT '-------------SELECT PROCEDURE FOR '+@TableSchema+'.'+@TableName+'-------------

'+@PROC_SELECT
	END

------------------------------------------FETCH NEXT TABLE FROM TABLE CURSOR

FETCH NEXT FROM MyAdventureTables_Cursor
	INTO @TableSchema,@TableName;
	WHILE (SELECT temporal_type
	FROM   sys.tables
	WHERE  object_id = OBJECT_ID(@TableSchema+'.'+@TableName, 'u'))=1
		BEGIN 
				FETCH NEXT FROM MyAdventureTables_Cursor
				INTO @TableSchema,@TableName;
		END
------------------------------------------CLOSE AND DEALLOCATE COLUMN CURSOR

CLOSE MyAdventureColumns_Cursor
DEALLOCATE MyAdventureColumns_Cursor

------------------------------------------END OF LOOP THROUGH ALL TABLES
		
END-- End of loop Through Tables

------------------------------------------COSE AND DEALLOCATE TABLES CURSOR

CLOSE MyAdventureTables_Cursor
DEALLOCATE MyAdventureTables_Cursor
		
GO
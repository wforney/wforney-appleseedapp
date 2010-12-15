using System;
using System.Data;
using System.Data.SqlClient;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Content.Data
{
    /// <summary>
    /// This class encapsulates the basic attributes of a Question, and is used
    /// by the administration pages when manipulating questions. QuestionItem implements
    /// the IComparable interface so that an ArrayList of QuestionItems may be sorted
    /// by TabOrder, using the ArrayList//s Sort() method.
    /// </summary>
    public class QuestionItem : IComparable
    {
        private int _QuestionOrder;
        private string _name;
        private int _id;
        private string _TypeOption;

        /// <summary>
        /// 
        /// </summary>
        public string TypeOption
        {
            get { return _TypeOption; }
            set { _TypeOption = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int QuestionOrder
        {
            get { return _QuestionOrder; }
            set { _QuestionOrder = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string QuestionName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int QuestionID
        {
            get { return _id; }
            set { _id = value; }
        }

//		public virtual int IComparable.CompareTo:CompareTo:(object value)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            int compareOrder = ((QuestionItem) value).QuestionOrder;

            if (QuestionOrder == compareOrder)
            {
                return 0;
            }
            if (QuestionOrder < compareOrder)
            {
                return -1;
            }
            if (QuestionOrder > compareOrder)
            {
                return 1;
            }
            return 0;
        }
    }


    /// <summary>
    /// This class encapsulates the basic attributes of an Option, and is used
    /// by the administration pages when manipulating questions/options.  OptionItem implements 
    /// the IComparable interface so that an ArrayList of OptionItems may be sorted
    /// by TabOrder, using the ArrayList//s Sort() method.
    /// </summary>
    public class OptionItem : IComparable
    {
        private int _OptionOrder;
        private string _name;
        private int _id;

        /// <summary>
        /// 
        /// </summary>
        public int OptionOrder
        {
            get { return _OptionOrder; }
            set { _OptionOrder = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OptionName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int OptionID
        {
            get { return _id; }
            set { _id = value; }
        }


//		public virtual int : IComparable.CompareTo CompareTo( object value)  // JLH!!
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            int compareOrder = ((OptionItem) value).OptionOrder;

            if (OptionOrder == compareOrder)
            {
                return 0;
            }
            if (OptionOrder < compareOrder)
            {
                return -1;
            }
            if (OptionOrder > compareOrder)
            {
                return 1;
            }
            return 0;
        }
    }


    /// <summary>
    /// IBS Tasks module
    /// Class that encapsulates all data logic necessary to add/query/delete
    /// surveys within the Portal database.
    /// Moved into Appleseed by Jakob Hansen
    /// </summary>
    public class SurveyDB
    {
        /// <summary>
        /// The GetQuestions method returns a SqlDataReader containing all of the
        /// questions for a specific portal module.
        /// Other relevant sources:
        /// GetSurveyQuestions Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        public SqlDataReader GetQuestions(int moduleID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyQuestions", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            // Execute the command and return the datareader 
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return result;
        }


        /// <summary>
        /// The GetOptions method returns a SqlDataReader containing all of the
        /// options for a specific portal module.
        /// Other relevant sources:
        /// GetSurveyOptions Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="TypeOption">The type option.</param>
        /// <returns></returns>
        public SqlDataReader GetOptions(int moduleID, string TypeOption)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyOptions", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterTypeOption = new SqlParameter("@TypeOption", SqlDbType.NVarChar, 2);
            parameterTypeOption.Value = TypeOption;
            myCommand.Parameters.Add(parameterTypeOption);

            // Execute the command and return the datareader 
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return result;
        }


        /// <summary>
        /// The GetOptionList method returns a SqlDataReader containing all of the
        /// options for a specific QuestionID.
        /// Other relevant sources:
        /// GetSurveyOptionList Stored Procedure
        /// </summary>
        /// <param name="QuestionID">The question ID.</param>
        /// <returns></returns>
        public SqlDataReader GetOptionList(int QuestionID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyOptionList", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            // Execute the command and return the datareader 
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return result;
        }


        /// <summary>
        /// The GetAnswers method returns a SqlDataReader containing all of the
        /// answers for a specific SurveyID.
        /// Other relevant sources:
        /// rb_GetSurveyAnswers Stored Procedure
        /// </summary>
        /// <param name="SurveyID">The survey ID.</param>
        /// <returns></returns>
        public SqlDataReader GetAnswers(int SurveyID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyAnswers", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterSurveyID = new SqlParameter("@SurveyID", SqlDbType.Int, 4);
            parameterSurveyID.Value = SurveyID;
            myCommand.Parameters.Add(parameterSurveyID);

            // Execute the command and return the datareader 
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return result;
        }


        /// <summary>
        /// The GetSurveyID method returns the SurveyID from rb_Surveys table
        /// for a specific ModuleID.
        /// Other relevant sources:
        /// rb_GetSurveyID Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <returns></returns>
        public int GetSurveyID(int ModuleID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyID", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterSurveyID = new SqlParameter("@SurveyID", SqlDbType.Int, 4);
            parameterSurveyID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterSurveyID);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterSurveyID.Value;
        }


        /// <summary>
        /// The GetQuestionList method returns a SqlDataReader containing all of the
        /// questions for a specific SurveyID.
        /// Other relevant sources:
        /// GetSurveyQuestionList Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <returns></returns>
        public SqlDataReader GetQuestionList(int ModuleID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyQuestionList", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            // Execute the command and return the datareader 
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return result;
        }


        /// <summary>
        /// The AddAnswer method add a record in rb_SurveyAnswers table
        /// for a specific SurveyID and QuestionID.
        /// Other relevant sources:
        /// rb_AddSurveyAnswer Stored Procedure
        /// </summary>
        /// <param name="SurveyID">The survey ID.</param>
        /// <param name="QuestionID">The question ID.</param>
        /// <param name="OptionID">The option ID.</param>
        /// <returns></returns>
        public int AddAnswer(int SurveyID, int QuestionID, int OptionID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_AddSurveyAnswer", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterSurveyID = new SqlParameter("@SurveyID", SqlDbType.Int, 4);
            parameterSurveyID.Value = SurveyID;
            myCommand.Parameters.Add(parameterSurveyID);

            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            SqlParameter parameterOptionID = new SqlParameter("@OptionID", SqlDbType.Int, 4);
            parameterOptionID.Value = OptionID;
            myCommand.Parameters.Add(parameterOptionID);

            SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int, 4);
            parameterItemID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterItemID);
            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterItemID.Value;
        }


        /// <summary>
        /// The AddQuestion method add a record in rb_SurveyQuestions table
        /// for a specific SurveyID.
        /// Other relevant sources:
        /// rb_AddSurveyQuestion Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="Question">The question.</param>
        /// <param name="ViewOrder">The view order.</param>
        /// <param name="TypeOption">The type option.</param>
        /// <returns></returns>
        public int AddQuestion(int ModuleID, string Question, int ViewOrder, string TypeOption)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_AddSurveyQuestion", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterQuestion = new SqlParameter("@Question", SqlDbType.NVarChar, 500);
            parameterQuestion.Value = Question;
            myCommand.Parameters.Add(parameterQuestion);

            SqlParameter parameterViewOrder = new SqlParameter("@ViewOrder", SqlDbType.Int, 4);
            parameterViewOrder.Value = ViewOrder;
            myCommand.Parameters.Add(parameterViewOrder);

            SqlParameter parameterTypeOption = new SqlParameter("@TypeOption", SqlDbType.NVarChar, 2);
            parameterTypeOption.Value = TypeOption;
            myCommand.Parameters.Add(parameterTypeOption);

            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterQuestionID);
            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterQuestionID.Value;
        }


        /// <summary>
        /// The AddOption method add a record in rb_SurveyOptions table
        /// for a specific QuestionID.
        /// Other relevant sources:
        /// rb_AddSurveyOption Stored Procedure
        /// </summary>
        /// <param name="QuestionID">The question ID.</param>
        /// <param name="OptionDesc">The option desc.</param>
        /// <param name="ViewOrder">The view order.</param>
        /// <returns></returns>
        public int AddOption(int QuestionID, string OptionDesc, int ViewOrder)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_AddSurveyOption", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            SqlParameter parameterOptionDesc = new SqlParameter("@OptionDesc", SqlDbType.NVarChar, 500);
            parameterOptionDesc.Value = OptionDesc;
            myCommand.Parameters.Add(parameterOptionDesc);

            SqlParameter parameterViewOrder = new SqlParameter("@ViewOrder", SqlDbType.Int, 4);
            parameterViewOrder.Value = ViewOrder;
            myCommand.Parameters.Add(parameterViewOrder);

            SqlParameter parameterOptionID = new SqlParameter("@OptionID", SqlDbType.Int, 4);
            parameterOptionID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterOptionID);
            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterOptionID.Value;
        }


        /// <summary>
        /// The DelQuestion method delete a record in rb_SurveyQuestions table
        /// for a specific QuestionID.
        /// Other relevant sources:
        /// rb_DelSurveyQuestion Stored Procedure
        /// </summary>
        /// <param name="QuestionID">The question ID.</param>
        /// <returns></returns>
        public int DelQuestion(int QuestionID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_DelSurveyQuestion", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return 1;
        }


        /// <summary>
        /// The DelOption method delete a record in rb_SurveyOptions table
        /// for a specific OptionID.
        /// Other relevant sources:
        /// rb_DelSurveyOption Stored Procedure
        /// </summary>
        /// <param name="OptionID">The option ID.</param>
        /// <returns></returns>
        public int DelOption(int OptionID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_DelSurveyOption", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterOptionID = new SqlParameter("@OptionID", SqlDbType.Int, 4);
            parameterOptionID.Value = OptionID;
            myCommand.Parameters.Add(parameterOptionID);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return 1;
        }


        /// <summary>
        /// The UpdateQuestionOrder method set the new ViewOrder in the
        /// rb_SurveyQuestions table for a specific QuestionID.
        /// Other relevant sources:
        /// rb_UpdateSurveyQuestionOrder Stored Procedure
        /// </summary>
        /// <param name="QuestionID">The question ID.</param>
        /// <param name="Order">The order.</param>
        /// <returns></returns>
        public int UpdateQuestionOrder(int QuestionID, int Order)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_UpdateSurveyQuestionOrder", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            SqlParameter parameterOrder = new SqlParameter("@Order", SqlDbType.Int, 4);
            parameterOrder.Value = Order;
            myCommand.Parameters.Add(parameterOrder);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return 1;
        }


        /// <summary>
        /// The UpdateOptionOrder method set the new ViewOrder in the
        /// rb_SurveyOptions table for a specific OptionID.
        /// Other relevant sources:
        /// rb_UpdateSurveyOptionOrder Stored Procedure
        /// </summary>
        /// <param name="OptionID">The option ID.</param>
        /// <param name="Order">The order.</param>
        /// <returns></returns>
        public int UpdateOptionOrder(int OptionID, int Order)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_UpdateSurveyOptionOrder", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameteroptionID = new SqlParameter("@OptionID", SqlDbType.Int, 4);
            parameteroptionID.Value = OptionID;
            myCommand.Parameters.Add(parameteroptionID);

            SqlParameter parameterOrder = new SqlParameter("@Order", SqlDbType.Int, 4);
            parameterOrder.Value = Order;
            myCommand.Parameters.Add(parameterOrder);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return 1;
        }


        /// <summary>
        /// The GetAnswerNum method get the number of answers
        /// for a specific SurveyID and QuestionID.
        /// Other relevant sources:
        /// rb_GetSurveyAnswersNum Stored Procedure
        /// </summary>
        /// <param name="SurveyID">The survey ID.</param>
        /// <param name="QuestionID">The question ID.</param>
        /// <returns></returns>
        public int GetAnswerNum(int SurveyID, int QuestionID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyAnswersNum", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterSurveyID = new SqlParameter("@SurveyID", SqlDbType.Int, 4);
            parameterSurveyID.Value = SurveyID;
            myCommand.Parameters.Add(parameterSurveyID);

            SqlParameter parameterQuestionID = new SqlParameter("@QuestionID", SqlDbType.Int, 4);
            parameterQuestionID.Value = QuestionID;
            myCommand.Parameters.Add(parameterQuestionID);

            SqlParameter parameterNum = new SqlParameter("@NumAnswer", SqlDbType.Int, 4);
            parameterNum.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterNum);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterNum.Value;
        }


        /// <summary>
        /// The ExistSurvey method checks whether the Survey exists in rb_Surveys
        /// table for a specific ModuleID.
        /// Other relevant sources:
        /// rb_ExistSurvey Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <returns></returns>
        public int ExistSurvey(int ModuleID)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_ExistSurvey", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterRowCount = new SqlParameter("@RowCount", SqlDbType.Int, 4);
            parameterRowCount.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterRowCount);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterRowCount.Value;
        }


        /// <summary>
        /// The ExistAddSurvey method checks whether the Survey exists in rb_Surveys
        /// table for a specific ModuleID, if not it creates a new one.
        /// Other relevant sources:
        /// rb_ExistAddSurvey Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="CreatedByUser">The created by user.</param>
        /// <returns></returns>
        public string ExistAddSurvey(int ModuleID, string CreatedByUser)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_ExistAddSurvey", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterCreatedByUser = new SqlParameter("@CreatedByUser", SqlDbType.NVarChar, 100);
            parameterCreatedByUser.Value = CreatedByUser;
            myCommand.Parameters.Add(parameterCreatedByUser);


            SqlParameter parameterSurveyDesc = new SqlParameter("@SurveyDesc", SqlDbType.NVarChar, 500);
            parameterSurveyDesc.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterSurveyDesc);


            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (string) parameterSurveyDesc.Value;
        }


        /// <summary>
        /// The GetDimArrays method get the dimensionof the arrays
        /// for a specific ModuleID and TypeOption.
        /// Other relevant sources:
        /// rb_GetSurveyDimArray Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="TypeOption">The type option.</param>
        /// <returns></returns>
        public int GetDimArray(int ModuleID, string TypeOption)
        {
            // Create Instance of Connection and Command object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSurveyDimArray", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = ModuleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterTypeOption = new SqlParameter("@TypeOption", SqlDbType.NChar, 2);
            parameterTypeOption.Value = TypeOption;
            myCommand.Parameters.Add(parameterTypeOption);

            SqlParameter parameterDimArray = new SqlParameter("@DimArray", SqlDbType.Int, 4);
            parameterDimArray.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterDimArray);


            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterDimArray.Value;
        }
    }
}
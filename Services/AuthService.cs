﻿using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Reflection.Metadata;
using TemplateAPI.Models;

namespace TemplateAPI.Services
{
    public interface IAuthService
    {
        public Task<Response> Register(User user);

        public Task<Response> Login(User user);

        public Task<Response> ValidateToken(string token);
    }
    public class AuthService : IAuthService
    {
        public readonly string _conString;

        public AuthService(string conString)
        {
            _conString = conString;
        }

        public async Task<Response> Login(User user)
        {
            var response = new Response();
            User loggedUser = new User();
            try
            {
                using (SqlConnection con = new SqlConnection(_conString))
                {
                    string query = "[dbo].[NSP_AuthUser]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Flag", "Get User");
                        cmd.Parameters.AddWithValue("@EMAIL", user.Email);
                        cmd.Parameters.AddWithValue("@PASSWORD", user.Password);


                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                loggedUser.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                                loggedUser.Email = reader["EMAIL"].ToString();
                                loggedUser.System = reader["SYSTEM"].ToString();
                                loggedUser.Token = reader["TOKEN"].ToString();
                                loggedUser.Timestamp = DateTime.Parse(reader["TIMESTAMP"].ToString());
                            }

                            Debug.WriteLine(response.Message);
                            if (string.IsNullOrEmpty(loggedUser.Email))
                            {
                                response.StatusCode = 401;
                                response.Message = "Incorrect login credentials";
                                response.IsError = true;
                                response.Body = loggedUser;
                                return response;
                            }

                            response.StatusCode = 201;
                            response.Message = "Logged in Success";
                            response.IsError = false;
                            response.Body = loggedUser;
                            return response;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                response.IsError = true;
                response.Message = Ex.Message;
                response.StatusCode = 501;
                return response;
            }
        }

        public async Task<Response> Register(User user)
        {
            var response = new Response();
            try
            {
                using (SqlConnection con = new SqlConnection(_conString))
                {
                    string query = "[dbo].[NSP_AuthUser]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Flag", "Create User");
                        cmd.Parameters.AddWithValue("@EMAIL", user.Email);
                        cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
                        cmd.Parameters.AddWithValue("@SYSTEM", user.System);

                        int isCreated = cmd.ExecuteNonQuery();

                        if (isCreated == 0)
                        {
                            response.StatusCode = 401;
                            response.Message = "Failed to register this user.";
                            response.IsError = true;
                            return response;
                        }

                        response.StatusCode = 201;
                        response.Message = "Successfully created a user.";
                        response.IsError = false;
                        return response;

                    }
                }
            }
            catch (SqlException Ex)
            {
                if (Ex.Number == 2601 || Ex.Number == 2627)
                {
                    response.Message = "Email already exist.";
                    response.IsError = true;
                    response.StatusCode = 501;
                    return response;
                }

                response.Message = Ex.Message;
                response.IsError = true;
                response.StatusCode = 501;
                return response;
            }
        }

        public async Task<Response> ValidateToken(string token)
        {
            string _token = "";
            var response = new Response();
            try
            {
                string query = "[dbo].[NSP_AuthUser]";

                using (SqlConnection con = new SqlConnection(_conString))
                {
                    await con.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Flag", "Validate Token");
                        cmd.Parameters.AddWithValue("@TOKEN", token);

                        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            // Check if DataTable is empty
                            if (dt.Rows.Count == 0)
                            {
                                throw new Exception("Invalid Token.");
                            }

                            response.IsError = false;
                            response.StatusCode = 200;
                            response.Message = "Authorized";

                            return response;

                        }
                    }

                }
            }
            catch (SqlException Ex)
            {
                response.IsError = true;
                response.StatusCode = 501;
                response.Message = Ex.Message;
            }
            catch (Exception Ex)
            {
                response.IsError = true;
                response.StatusCode = 501;
                response.Message = Ex.Message;
            }

            return response;
        }
    }

}


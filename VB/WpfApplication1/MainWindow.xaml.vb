﻿Imports Microsoft.VisualBasic
Imports System.Windows
Imports System.Data
Imports System.Data.OleDb
Imports DevExpress.XtraScheduler

Namespace WpfApplication1
    Partial Public Class MainWindow
        Inherits Window

        Private dataSet As CarsDBDataSet
        Private adapter As CarsDBDataSetTableAdapters.CarSchedulingTableAdapter

        Public Sub New()
            InitializeComponent()

            schedulerControl1.Start = New System.DateTime(2010, 7, 15, 0, 0, 0, 0)

            Me.dataSet = New CarsDBDataSet()

            ' Bind the scheduler storage to appointments data.
            Me.schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling

            ' Load data into the 'CarsDBDataSet.CarScheduling' table. 
            Me.adapter = New CarsDBDataSetTableAdapters.CarSchedulingTableAdapter()
            Me.adapter.Fill(dataSet.CarScheduling)

            AddHandler schedulerControl1.Storage.AppointmentsInserted,
                AddressOf Storage_AppointmentsModified
            AddHandler schedulerControl1.Storage.AppointmentsChanged,
                AddressOf Storage_AppointmentsModified
            AddHandler schedulerControl1.Storage.AppointmentsDeleted,
                AddressOf Storage_AppointmentsModified

            AddHandler adapter.Adapter.RowUpdated,
                AddressOf adapter_RowUpdated
        End Sub

        Private Sub Storage_AppointmentsModified(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            Me.adapter.Adapter.Update(Me.dataSet)
            Me.dataSet.AcceptChanges()

        End Sub

        Private Sub adapter_RowUpdated(ByVal sender As Object, ByVal e As OleDbRowUpdatedEventArgs)
            If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
                Dim id As Integer = 0
                Using cmd As New OleDbCommand("SELECT @@IDENTITY", adapter.Connection)
                    id = CInt(Fix(cmd.ExecuteScalar()))
                End Using
                e.Row("ID") = id
            End If
        End Sub
    End Class
End Namespace
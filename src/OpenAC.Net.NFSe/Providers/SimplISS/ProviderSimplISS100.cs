﻿// ***********************************************************************
// Assembly         : OpenAC.Net.NFSe
// Author           : Rafael Dias
// Created          : 17-02-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 17-02-2020
// ***********************************************************************
// <copyright file="ProviderSimplISS.cs" company="OpenAC .Net">
//		        		   The MIT License (MIT)
//	     		Copyright (c) 2014 - 2024 Projeto OpenAC .Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.DFe.Core.Serializer;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Interface;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Configuracao;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Providers;

internal sealed class ProviderSimplISS100 : ProviderABRASF
{
    #region Constructors

    public ProviderSimplISS100(ConfigNFSe config, OpenMunicipioNFSe municipio) : base(config, municipio)
    {
        Name = "SimplISS";
    }

    #endregion Constructors

    #region RPS

    protected override void LoadServicosValoresRps(NotaServico nota, XElement rootNFSe)
    {
        base.LoadServicosValoresRps(nota, rootNFSe);
        var rootServico = rootNFSe.ElementAnyNs("Servico");
        if (rootServico == null) return;

        var items = rootServico.ElementsAnyNs("ItensServico");
        foreach (var item in items)
        {
            var servico = nota.Servico.ItemsServico.AddNew();
            servico.Descricao = item.ElementAnyNs("Descricao")?.GetValue<string>() ?? "";
            servico.Quantidade = item.ElementAnyNs("Quantidade")?.GetValue<decimal>() ?? 0;
            servico.ValorUnitario = item.ElementAnyNs("ValorUnitario")?.GetValue<decimal>() ?? 0;
        }
    }

    protected override XElement WriteServicosValoresRps(NotaServico nota)
    {
        var servico = new XElement("Servico");
        var valores = new XElement("Valores");
        servico.AddChild(valores);

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

        valores.AddChild(AddTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
        valores.AddChild(AddTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
        valores.AddChild(AddTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));

        valores.AddChild(AddTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
        valores.AddChild(AddTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
        valores.AddChild(AddTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

        servico.AddChild(AddTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

        servico.AddChild(AddTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

        servico.AddChild(AddTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
        servico.AddChild(AddTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
        servico.AddChild(AddTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

        foreach (var item in nota.Servico.ItemsServico)
        {
            var itemServico = new XElement("ItensServico");
            itemServico.AddChild(AddTag(TipoCampo.Str, "", "Descricao", 1, 100, Ocorrencia.Obrigatoria, item.Descricao));
            itemServico.AddChild(AddTag(TipoCampo.De2, "", "Quantidade", 4, 15, Ocorrencia.Obrigatoria, item.Quantidade));
            itemServico.AddChild(AddTag(TipoCampo.De2, "", "ValorUnitario", 4, 15, Ocorrencia.Obrigatoria, item.ValorUnitario));

            servico.AddChild(itemServico);
        }

        return servico;
    }

    #endregion RPS

    #region Methods

    protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas) => throw new NotImplementedException("Função não implementada/suportada neste Provedor !");

    protected override string GetNamespace() => "";

    protected override IServiceClient GetClient(TipoUrl tipo) => new SimplISS100ServiceClient(this, tipo);

    protected override string GetSchema(TipoUrl tipo) => "nfse_3.xsd";

    protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResult");
        var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");

        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        retornoWebservice.Sucesso = true;

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
            var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            var dataEmissao = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
            var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataEmissao);

            var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            if (nota == null)
            {
                notas.Load(compNfse.ToString());
            }
            else
            {
                nota.IdentificacaoNFSe.Numero = numeroNFSe;
                nota.IdentificacaoNFSe.Chave = chaveNFSe;
                nota.IdentificacaoNFSe.DataEmissao = dataEmissao;
                nota.XmlOriginal = compNfse.AsString();
            }
            
        }
        retornoWebservice.Notas = [.. notas];
    }

    protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var compNfse = xmlRet.ElementAnyNs("ConsultarNfsePorRpsResult")?.ElementAnyNs("CompNfse");
        if (compNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
            return;
        }

        var infNFSe = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
        var numeroNFSe = infNFSe.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
        var chaveNFSe = infNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
        var dataNFSe = infNFSe.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
        var numeroRps = infNFSe.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;

        GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

        var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
        if (nota == null)
        {
            nota = notas.Load(compNfse.ToString());
        }
        else
        {
            nota.IdentificacaoNFSe.Numero = numeroNFSe;
            nota.IdentificacaoNFSe.Chave = chaveNFSe;
            nota.IdentificacaoNFSe.DataEmissao = dataNFSe;
            nota.XmlOriginal = compNfse.ToString();
        }

        retornoWebservice.Nota = nota;
        retornoWebservice.Sucesso = true;
    }

    /// <inheritdoc />
    protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var retornoLote = xmlRet.ElementAnyNs("ConsultarNfseResult");
        var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
        if (listaNfse == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
            return;
        }

        var notasServico = new List<NotaServico>();

        foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
        {
            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);
            notasServico.Add(nota);
        }

        retornoWebservice.Notas = notasServico.ToArray();
        retornoWebservice.Sucesso = true;
    }

    protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
    {
        // Analisa mensagem de retorno
        var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
        MensagemErro(retornoWebservice, xmlRet.Root);
        if (retornoWebservice.Erros.Any()) return;

        var confirmacaoCancelamento = xmlRet.Root.ElementAnyNs("Cancelamento")?
            .ElementAnyNs("Confirmacao");

        if (confirmacaoCancelamento == null)
        {
            retornoWebservice.Erros.Add(new EventoRetorno { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
            return;
        }

        retornoWebservice.Data = confirmacaoCancelamento.ElementAnyNs("DataHoraCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
        retornoWebservice.Sucesso = retornoWebservice.Data != DateTime.MinValue;
        retornoWebservice.CodigoCancelamento = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")
            .ElementAnyNs("CodigoCancelamento").GetValue<string>();

        var numeroNFSe = confirmacaoCancelamento.ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?
            .ElementAnyNs("IdentificacaoNfse")?.ElementAnyNs("Numero").GetValue<string>() ?? string.Empty;

        // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
        var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
        if (nota == null) return;

        nota.Situacao = SituacaoNFSeRps.Cancelado;
        nota.Cancelamento.Pedido.CodigoCancelamento = retornoWebservice.CodigoCancelamento;
        nota.Cancelamento.DataHora = retornoWebservice.Data;
    }

    #endregion Methods
}
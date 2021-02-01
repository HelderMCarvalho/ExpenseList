-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 31-Jan-2021 às 21:02
-- Versão do servidor: 10.3.16-MariaDB
-- versão do PHP: 7.3.7

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `despesas_isi`
--

-- --------------------------------------------------------

--
-- Estrutura da tabela `despesas`
--

CREATE TABLE `despesas` (
  `id` int(11) NOT NULL COMMENT 'Id da despesa',
  `nome` varchar(65) NOT NULL COMMENT 'Nome da despesa',
  `descricao` text NOT NULL COMMENT 'Descrição da despesa',
  `dataHoraCriacao` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp() COMMENT 'Data e hora de criação da despesa',
  `valEur` decimal(65,2) NOT NULL COMMENT 'Valor da despesa em EUR',
  `valUsd` decimal(65,2) NOT NULL COMMENT 'Valor da despesa em USD',
  `utilizador_id` varchar(64) NOT NULL COMMENT 'Id do utilizador que criou a despesa'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Tabela de despesas';

-- --------------------------------------------------------

--
-- Estrutura da tabela `utilizadores`
--

CREATE TABLE `utilizadores` (
  `emailSha` varchar(64) NOT NULL COMMENT 'Hash do email',
  `moedaPadrao` varchar(3) NOT NULL COMMENT 'Moeda padrão (EUR/USD)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Tabela de Utilizadores';

--
-- Índices para tabelas despejadas
--

--
-- Índices para tabela `despesas`
--
ALTER TABLE `despesas`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Despesas por Utilizador` (`utilizador_id`);

--
-- Índices para tabela `utilizadores`
--
ALTER TABLE `utilizadores`
  ADD PRIMARY KEY (`emailSha`);

--
-- AUTO_INCREMENT de tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `despesas`
--
ALTER TABLE `despesas`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Id da despesa';

--
-- Restrições para despejos de tabelas
--

--
-- Limitadores para a tabela `despesas`
--
ALTER TABLE `despesas`
  ADD CONSTRAINT `Despesas por Utilizador` FOREIGN KEY (`utilizador_id`) REFERENCES `utilizadores` (`emailSha`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
